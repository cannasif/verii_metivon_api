using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Modules.AccessControl.Authorization;

public static class PermissionPolicy
{
    public const string Prefix = "Permission:";
    public static string Name(string code) => Prefix + code.Trim().ToLowerInvariant();
}

public sealed class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string code) => Policy = PermissionPolicy.Name(code);
}

public sealed record PermissionRequirement(string Code) : IAuthorizationRequirement;

public interface IPermissionService
{
    Task<MyPermissionsResult?> GetSnapshotAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(ClaimsPrincipal principal, string code, CancellationToken cancellationToken = default);
}

public sealed class PermissionService(MetivonDbContext db) : IPermissionService
{
    private static bool IsRoleAdmin(string? role) => role is not null &&
        (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || role.Equals("SystemAdmin", StringComparison.OrdinalIgnoreCase) || role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase));

    public async Task<MyPermissionsResult?> GetSnapshotAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return null;
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId && x.IsActive, cancellationToken);
        if (user is null) return null;
        var now = DateTime.UtcNow;
        var assignments = await db.Set<UserPermissionGroup>().AsNoTracking()
            .Where(x => x.UserId == userId && x.PermissionGroup.IsActive &&
                        (!x.ValidFrom.HasValue || x.ValidFrom <= now) && (!x.ValidUntil.HasValue || x.ValidUntil >= now))
            .Include(x => x.PermissionGroup).ThenInclude(x => x.GroupPermissions).ThenInclude(x => x.PermissionDefinition)
            .ToListAsync(cancellationToken);
        var isSystemAdmin = IsRoleAdmin(user.Role) || assignments.Any(x => x.PermissionGroup.IsSystemAdmin);
        var denied = assignments.SelectMany(x => x.PermissionGroup.GroupPermissions)
            .Where(x => x.IsDenied && x.PermissionDefinition.IsActive).Select(x => x.PermissionDefinition.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var allowed = assignments.SelectMany(x => x.PermissionGroup.GroupPermissions)
            .Where(x => !x.IsDenied && x.PermissionDefinition.IsActive).Select(x => x.PermissionDefinition.Code)
            .Where(x => !denied.Contains(x)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray();
        return new MyPermissionsResult(user.Id, user.Role, isSystemAdmin,
            assignments.Select(x => x.PermissionGroup.Name).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray(), allowed);
    }

    public async Task<bool> HasPermissionAsync(ClaimsPrincipal principal, string code, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetSnapshotAsync(principal, cancellationToken);
        return snapshot is not null && (snapshot.IsSystemAdmin || snapshot.PermissionCodes.Contains(code, StringComparer.OrdinalIgnoreCase));
    }
}

public sealed class PermissionAuthorizationHandler(IPermissionService permissions) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (await permissions.HasPermissionAsync(context.User, requirement.Code)) context.Succeed(requirement);
    }
}

public sealed class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PermissionPolicy.Prefix, StringComparison.OrdinalIgnoreCase)) return _fallback.GetPolicyAsync(policyName);
        var code = policyName[PermissionPolicy.Prefix.Length..];
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().AddRequirements(new PermissionRequirement(code)).Build();
        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}

public interface IAccessControlAuditWriter
{
    Task WriteAsync(string action, string entityType, string? entityId, object? oldValue, object? newValue, CancellationToken cancellationToken = default);
}

public sealed class AccessControlAuditWriter(MetivonDbContext db, IHttpContextAccessor accessor) : IAccessControlAuditWriter
{
    public async Task WriteAsync(string action, string entityType, string? entityId, object? oldValue, object? newValue, CancellationToken cancellationToken = default)
    {
        var http = accessor.HttpContext;
        long? userId = long.TryParse(http?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsed) ? parsed : null;
        await db.Set<AccessControlAuditLog>().AddAsync(new AccessControlAuditLog
        {
            TraceId = http?.TraceIdentifier ?? Guid.NewGuid().ToString("N"), ActionType = action, EntityType = entityType, EntityId = entityId,
            PerformedByUserId = userId, PerformedByUserEmail = http?.User.FindFirstValue(ClaimTypes.Email),
            RequestPath = http?.Request.Path, RequestMethod = http?.Request.Method,
            OldValuesJson = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            NewValuesJson = newValue is null ? null : JsonSerializer.Serialize(newValue)
        }, cancellationToken);
    }
}
