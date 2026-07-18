using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Modules.Identity.Api;

[ApiController, Authorize, Route("api/User")]
public sealed class UsersController(MetivonDbContext db) : ControllerBase
{
    [HttpPost("query")]
    public async Task<IActionResult> Query(UserQuery request, CancellationToken ct)
    {
        var query = db.Users.AsNoTracking().Include(x => x.Detail).Include(x => x.ManagerUser).ThenInclude(x => x!.Detail).AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.Username.Contains(search) || x.Email.Contains(search) ||
                (x.Detail != null && ((x.Detail.FirstName != null && x.Detail.FirstName.Contains(search)) || (x.Detail.LastName != null && x.Detail.LastName.Contains(search)))));
        }
        foreach (var filter in request.Filters.Take(50))
        {
            if (string.IsNullOrWhiteSpace(filter.Value)) continue;
            var value = filter.Value.Trim();
            switch (filter.Column.Trim().ToLowerInvariant())
            {
                case "id" when long.TryParse(value, out var id): query = query.Where(x => x.Id == id); break;
                case "username": query = query.Where(x => x.Username.Contains(value)); break;
                case "email": query = query.Where(x => x.Email.Contains(value)); break;
                case "role": query = query.Where(x => x.Role.Contains(value)); break;
                case "isactive" or "status" when bool.TryParse(value, out var active): query = query.Where(x => x.IsActive == active); break;
            }
        }

        var total = await query.CountAsync(ct);
        query = (request.SortBy ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "username" => request.IsDescending ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username),
            "email" => request.IsDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
            "role" => request.IsDescending ? query.OrderByDescending(x => x.Role) : query.OrderBy(x => x.Role),
            "isactive" or "status" => request.IsDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
            "creationtime" => request.IsDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            _ => request.IsDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
        };
        var users = await query.Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        var rows = await MapManyAsync(users, ct);
        return Ok(ApiResponse<PagedResult<UserDto>>.Ok(new(rows, request.NormalizedPageNumber, request.NormalizedPageSize, total)));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var entity = await LoadAsync(id, false, ct);
        return entity is null ? NotFound(ApiResponse<object>.Error("User was not found.", 404)) : Ok(ApiResponse<UserDto>.Ok(await MapAsync(entity, ct)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken ct)
    {
        var username = request.Username.Trim(); var email = request.Email.Trim().ToLowerInvariant();
        if (username.Length is < 3 or > 100 || !email.Contains('@')) return BadRequest(ApiResponse<object>.Error("A valid username and email are required.", 400));
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8) return BadRequest(ApiResponse<object>.Error("Password must contain at least 8 characters.", 400));
        if (await db.Users.AnyAsync(x => x.Username == username || x.Email == email, ct)) return Conflict(ApiResponse<object>.Error("Username or email already exists.", 409));
        var groups = await ResolveGroupsAsync(request.RoleId, request.PermissionGroupIds, ct);
        if (groups is null) return BadRequest(ApiResponse<object>.Error("One or more permission groups are invalid.", 400));
        if (request.ManagerUserId.HasValue && !await db.Users.AnyAsync(x => x.Id == request.ManagerUserId && x.IsActive, ct)) return BadRequest(ApiResponse<object>.Error("Manager user is invalid.", 400));
        var branchId = ResolveBranchId();
        if (!await db.Branches.AnyAsync(x => x.Id == branchId && x.IsActive, ct)) branchId = await db.Branches.Where(x => x.IsActive).OrderByDescending(x => x.IsDefault).Select(x => x.Id).FirstAsync(ct);

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var primary = groups.OrderBy(x => x.Priority).ThenBy(x => x.Id).First();
        var entity = new User { Username = username, Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), BranchId = branchId, ManagerUserId = request.ManagerUserId, Role = primary.IsSystemAdmin ? "Admin" : primary.Code, IsActive = request.IsActive ?? true,
            Detail = new UserDetail { FirstName = Clean(request.FirstName), LastName = Clean(request.LastName), PhoneNumber = Clean(request.PhoneNumber) } };
        db.Users.Add(entity); await db.SaveChangesAsync(ct);
        db.UserPermissionGroups.AddRange(groups.Select(x => new UserPermissionGroup { UserId = entity.Id, PermissionGroupId = x.Id }));
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return StatusCode(201, ApiResponse<UserDto>.Ok(await MapAsync(entity, ct), "User created."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateUserRequest request, CancellationToken ct)
    {
        var entity = await LoadAsync(id, true, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("User was not found.", 404));
        if (request.ManagerUserId == id) return BadRequest(ApiResponse<object>.Error("A user cannot be their own manager.", 400));
        if (request.ManagerUserId.HasValue && !await db.Users.AnyAsync(x => x.Id == request.ManagerUserId && x.IsActive, ct)) return BadRequest(ApiResponse<object>.Error("Manager user is invalid.", 400));
        if (!string.IsNullOrWhiteSpace(request.Email)) { var email = request.Email.Trim().ToLowerInvariant(); if (!email.Contains('@')) return BadRequest(ApiResponse<object>.Error("Email is invalid.", 400)); if (await db.Users.AnyAsync(x => x.Id != id && x.Email == email, ct)) return Conflict(ApiResponse<object>.Error("Email already exists.", 409)); entity.Email = email; }
        var groups = await ResolveGroupsAsync(request.RoleId, request.PermissionGroupIds, ct); if (groups is null) return BadRequest(ApiResponse<object>.Error("One or more permission groups are invalid.", 400));

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        entity.Detail ??= new UserDetail { UserId = entity.Id };
        entity.Detail.FirstName = request.FirstName is null ? entity.Detail.FirstName : Clean(request.FirstName);
        entity.Detail.LastName = request.LastName is null ? entity.Detail.LastName : Clean(request.LastName);
        entity.Detail.PhoneNumber = request.PhoneNumber is null ? entity.Detail.PhoneNumber : Clean(request.PhoneNumber);
        entity.ManagerUserId = request.ManagerUserId; if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value; entity.UpdatedAt = DateTime.UtcNow;
        var primary = groups.OrderBy(x => x.Priority).ThenBy(x => x.Id).First(); entity.Role = primary.IsSystemAdmin ? "Admin" : primary.Code;
        var oldLinks = await db.UserPermissionGroups.Where(x => x.UserId == id).ToListAsync(ct); db.UserPermissionGroups.RemoveRange(oldLinks);
        db.UserPermissionGroups.AddRange(groups.Select(x => new UserPermissionGroup { UserId = id, PermissionGroupId = x.Id }));
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return Ok(ApiResponse<UserDto>.Ok(await MapAsync(entity, ct), "User updated."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        if (User.FindFirstValue(ClaimTypes.NameIdentifier) == id.ToString()) return Conflict(ApiResponse<object>.Error("The active user cannot delete their own account.", 409));
        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("User was not found.", 404));
        if (await db.Users.AnyAsync(x => x.ManagerUserId == id, ct)) return Conflict(ApiResponse<object>.Error("Assign direct reports to another manager before deleting this user.", 409));
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; entity.IsActive = false; entity.RefreshToken = null; entity.RefreshTokenExpiresAt = null; await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(new { id }, "User deleted."));
    }

    private long ResolveBranchId() => long.TryParse(User.FindFirstValue("branchId"), out var id) ? id : 0;
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private async Task<User?> LoadAsync(long id, bool tracking, CancellationToken ct) { IQueryable<User> query = db.Users; if (!tracking) query = query.AsNoTracking(); return await query.Include(x => x.Detail).Include(x => x.ManagerUser).ThenInclude(x => x!.Detail).FirstOrDefaultAsync(x => x.Id == id, ct); }
    private async Task<List<PermissionGroup>?> ResolveGroupsAsync(long? roleId, IReadOnlyList<long>? requested, CancellationToken ct)
    {
        var ids = (requested ?? []).Concat(roleId.HasValue ? [roleId.Value] : []).Where(x => x > 0).Distinct().ToArray();
        if (ids.Length == 0) return null;
        var groups = await db.PermissionGroups.Where(x => ids.Contains(x.Id) && x.IsActive).ToListAsync(ct); return groups.Count == ids.Length ? groups : null;
    }
    private async Task<IReadOnlyList<UserDto>> MapManyAsync(IReadOnlyList<User> users, CancellationToken ct)
    {
        var ids = users.Select(x => x.Id).ToArray();
        var roles = await db.UserPermissionGroups.AsNoTracking().Where(x => ids.Contains(x.UserId)).Include(x => x.PermissionGroup).OrderBy(x => x.PermissionGroup.Priority).ThenBy(x => x.PermissionGroupId).ToListAsync(ct);
        return users.Select(x => Map(x, roles.Where(y => y.UserId == x.Id).Select(y => y.PermissionGroup).ToArray())).ToArray();
    }
    private async Task<UserDto> MapAsync(User user, CancellationToken ct) => Map(user, await db.UserPermissionGroups.AsNoTracking().Where(x => x.UserId == user.Id).Include(x => x.PermissionGroup).OrderBy(x => x.PermissionGroup.Priority).ThenBy(x => x.PermissionGroupId).Select(x => x.PermissionGroup).ToArrayAsync(ct));
    private static UserDto Map(User x, IReadOnlyList<PermissionGroup> groups)
    {
        var primary = groups.FirstOrDefault(); var fullName = $"{x.Detail?.FirstName} {x.Detail?.LastName}".Trim(); var managerName = $"{x.ManagerUser?.Detail?.FirstName} {x.ManagerUser?.Detail?.LastName}".Trim();
        return new(x.Id, x.Username, x.Email, x.Detail?.FirstName, x.Detail?.LastName, x.Detail?.PhoneNumber, primary?.Name ?? x.Role, primary?.Id, x.ManagerUserId, string.IsNullOrWhiteSpace(managerName) ? x.ManagerUser?.Username : managerName, true, x.IsActive, x.LastLoginAt, string.IsNullOrWhiteSpace(fullName) ? x.Username : fullName, x.CreatedAt, x.UpdatedAt);
    }
}

public sealed class UserQuery : PagedQuery;
public sealed record CreateUserRequest(string Username, string Email, string? Password, string? FirstName, string? LastName, string? PhoneNumber, long RoleId, long? ManagerUserId, bool? IsActive, IReadOnlyList<long>? PermissionGroupIds);
public sealed record UpdateUserRequest(string? Email, string? FirstName, string? LastName, string? PhoneNumber, long? RoleId, long? ManagerUserId, bool? IsActive, IReadOnlyList<long>? PermissionGroupIds);
public sealed record UserDto(long Id, string Username, string Email, string? FirstName, string? LastName, string? PhoneNumber, string Role, long? RoleId, long? ManagerUserId, string? ManagerFullName, bool IsEmailConfirmed, bool IsActive, DateTime? LastLoginDate, string FullName, DateTime CreationTime, DateTime? LastModificationTime);
