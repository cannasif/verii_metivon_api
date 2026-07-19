using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Authorization;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Modules.AccessControl.Api;

[ApiController, Route("api/visibility-policies")]
public sealed class VisibilityPoliciesController(MetivonDbContext db, IPermissionService permissions, IAccessControlAuditWriter audit) : ControllerBase
{
    [HttpPost("query"), PermissionAuthorize("access-control.visibility-policies.view")]
    public async Task<IActionResult> Query(VisibilityPolicyQuery request, CancellationToken ct)
    {
        var query = db.Set<VisibilityPolicy>().AsNoTracking(); if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (!string.IsNullOrWhiteSpace(request.EntityType)) query = query.Where(x => x.EntityType == request.EntityType);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); query = query.Where(x => x.Code.Contains(s) || x.Name.Contains(s) || x.EntityType.Contains(s) || (x.Description != null && x.Description.Contains(s))); }
        query = query.ApplyPagedFilters(request); var total = await query.CountAsync(ct); var rows = await query.ApplyPagedSort(request, nameof(VisibilityPolicy.Code)).Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        return Ok(ApiResponse<PagedResult<VisibilityPolicyDto>>.Ok(new(rows.Select(Map).ToArray(), request.NormalizedPageNumber, request.NormalizedPageSize, total)));
    }

    [HttpGet("{id:long}"), PermissionAuthorize("access-control.visibility-policies.view")]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => await db.Set<VisibilityPolicy>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct) is { } item ? Ok(ApiResponse<VisibilityPolicyDto>.Ok(Map(item))) : NotFound(ApiResponse<object>.Error("Visibility policy was not found.", 404));

    [HttpPost, PermissionAuthorize("access-control.visibility-policies.create")]
    public async Task<IActionResult> Create(CreateVisibilityPolicyRequest request, CancellationToken ct)
    {
        if (!Enum.IsDefined(typeof(VisibilityScopeType), request.ScopeType)) return BadRequest(ApiResponse<object>.Error("Visibility scope is invalid.", 400)); var code = Normalize(request.Code);
        if (await db.Set<VisibilityPolicy>().AnyAsync(x => x.Code == code, ct)) return Conflict(ApiResponse<object>.Error("Visibility policy code already exists.", 409));
        var entity = new VisibilityPolicy { Code = code, Name = request.Name.Trim(), EntityType = request.EntityType.Trim(), Description = request.Description?.Trim(), ScopeType = (VisibilityScopeType)request.ScopeType, IncludeSelf = request.IncludeSelf, IsActive = request.IsActive, Priority = request.Priority };
        db.Add(entity); await audit.WriteAsync("Create", nameof(VisibilityPolicy), null, null, entity, ct); await db.SaveChangesAsync(ct); return StatusCode(201, ApiResponse<VisibilityPolicyDto>.Ok(Map(entity), "Visibility policy created."));
    }

    [HttpPut("{id:long}"), PermissionAuthorize("access-control.visibility-policies.update")]
    public async Task<IActionResult> Update(long id, UpdateVisibilityPolicyRequest request, CancellationToken ct)
    {
        var entity = await db.Set<VisibilityPolicy>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("Visibility policy was not found.", 404)); var old = Map(entity);
        if (request.Code is not null) entity.Code = Normalize(request.Code); if (request.Name is not null) entity.Name = request.Name.Trim(); if (request.EntityType is not null) entity.EntityType = request.EntityType.Trim(); if (request.Description is not null) entity.Description = request.Description.Trim();
        if (request.ScopeType.HasValue) { if (!Enum.IsDefined(typeof(VisibilityScopeType), request.ScopeType.Value)) return BadRequest(ApiResponse<object>.Error("Visibility scope is invalid.", 400)); entity.ScopeType = (VisibilityScopeType)request.ScopeType.Value; }
        if (request.IncludeSelf.HasValue) entity.IncludeSelf = request.IncludeSelf.Value; if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value; if (request.Priority.HasValue) entity.Priority = request.Priority.Value; entity.UpdatedAt = DateTime.UtcNow;
        await audit.WriteAsync("Update", nameof(VisibilityPolicy), id.ToString(), old, entity, ct); await db.SaveChangesAsync(ct); return Ok(ApiResponse<VisibilityPolicyDto>.Ok(Map(entity), "Visibility policy updated."));
    }

    [HttpDelete("{id:long}"), PermissionAuthorize("access-control.visibility-policies.delete")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await db.Set<VisibilityPolicy>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("Visibility policy was not found.", 404)); entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        await audit.WriteAsync("Delete", nameof(VisibilityPolicy), id.ToString(), entity, null, ct); await db.SaveChangesAsync(ct); return Ok(ApiResponse<object>.Ok(new { id }, "Visibility policy deleted."));
    }

    [HttpGet("preview"), PermissionAuthorize("access-control.visibility-simulator.view")]
    public async Task<IActionResult> Preview(long userId, string entityType, CancellationToken ct) => Ok(ApiResponse<VisibilityPreviewResult>.Ok(await BuildPreview(userId, entityType, ct)));

    [HttpGet("approval-audit"), PermissionAuthorize("access-control.visibility-simulator.view")]
    public IActionResult ApprovalAudit(long userId, string entityType) => Ok(ApiResponse<IReadOnlyList<object>>.Ok([]));

    [HttpGet("simulate"), PermissionAuthorize("access-control.visibility-simulator.view")]
    public async Task<IActionResult> Simulate(long userId, string entityType, long entityId, CancellationToken ct)
    {
        var preview = await BuildPreview(userId, entityType, ct); var canReach = preview.IsUnrestricted || preview.HasExplicitPolicy;
        var permissionSnapshot = await permissions.GetSnapshotAsync(User, ct); var prefix = EntityPermissionPrefix(entityType);
        bool Has(string action) => permissionSnapshot?.IsSystemAdmin == true || permissionSnapshot?.PermissionCodes.Contains($"{prefix}.{action}", StringComparer.OrdinalIgnoreCase) == true;
        var actions = new[] { "view", "create", "update", "delete" }.Select(action => new ActionSimulationResult(action, canReach && Has(action), canReach ? (Has(action) ? "Permission and data scope allow this action." : "The required action permission is missing.") : "The record is outside the user's data visibility scope.")).ToArray();
        return Ok(ApiResponse<VisibilityActionSimulationResult>.Ok(new(userId, entityType, entityId, actions)));
    }

    private async Task<VisibilityPreviewResult> BuildPreview(long userId, string entityType, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().Include(x => x.Detail).FirstOrDefaultAsync(x => x.Id == userId, ct) ?? throw new KeyNotFoundException("User was not found.");
        var policies = await db.Set<UserVisibilityPolicy>().AsNoTracking().Where(x => x.UserId == userId && x.VisibilityPolicy.IsActive && x.VisibilityPolicy.EntityType == entityType).Include(x => x.VisibilityPolicy).OrderBy(x => x.VisibilityPolicy.Priority).Select(x => x.VisibilityPolicy).ToListAsync(ct);
        var admin = user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || user.Role.Equals("SystemAdmin", StringComparison.OrdinalIgnoreCase) || user.Role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        var scopes = policies.Select(x => x.ScopeType).ToHashSet(); IQueryable<Core.Domain.User> visible = db.Users.AsNoTracking().Include(x => x.Detail).Where(x => x.IsActive);
        if (!admin && !scopes.Contains(VisibilityScopeType.Company))
        {
            if (scopes.Contains(VisibilityScopeType.Branch) || scopes.Contains(VisibilityScopeType.Warehouse)) visible = visible.Where(x => x.BranchId == user.BranchId);
            else if (scopes.Contains(VisibilityScopeType.PermissionGroup)) { var groupIds = db.Set<UserPermissionGroup>().Where(x => x.UserId == userId).Select(x => x.PermissionGroupId); visible = visible.Where(x => db.Set<UserPermissionGroup>().Any(g => g.UserId == x.Id && groupIds.Contains(g.PermissionGroupId))); }
            else visible = visible.Where(x => x.Id == userId);
        }
        var users = await visible.OrderBy(x => x.Username).Select(x => new VisibilityPreviewUser(x.Id, x.Detail != null && (x.Detail.FirstName != null || x.Detail.LastName != null) ? ((x.Detail.FirstName ?? "") + " " + (x.Detail.LastName ?? "")).Trim() : x.Username, x.Email)).ToListAsync(ct);
        if (policies.Any(x => x.IncludeSelf) && users.All(x => x.UserId != userId)) users.Add(new(userId, user.Username, user.Email));
        return new(userId, entityType, policies.Count > 0, admin || scopes.Contains(VisibilityScopeType.Company), users.Select(x => x.UserId).Distinct().ToArray(), users, policies.Select(x => new VisibilityPreviewPolicy(x.Id, x.Code, x.Name, (int)x.ScopeType, x.IncludeSelf)).ToArray(), [], []);
    }

    private static string EntityPermissionPrefix(string entityType) => entityType switch { "BusinessPartner" => "accounts.account-management", "Product" => "products.product-management", "Warehouse" => "warehouses.warehouse-management", "GoodsReceipt" => "receiving.goods-receipts", "TransferOrder" => "transfers.transfer-orders", "Shipment" => "shipping.shipments", "PurchaseOrder" => "procurement.purchase-orders", "SalesOrder" => "sales.sales-orders", "ImportDossier" => "landed-costs.import-dossiers", "ElectronicDocument" => "e-documents.e-documents", "Accounting" => "accounting.ledger-accounts", _ => entityType.ToLowerInvariant() };
    private static string Normalize(string value) => value.Trim().ToUpperInvariant().Replace(' ', '_');
    private static VisibilityPolicyDto Map(VisibilityPolicy x) => new(x.Id, x.CreatedAt, x.UpdatedAt, x.DeletedAt, x.IsDeleted, x.Code, x.Name, x.EntityType, x.Description, (int)x.ScopeType, x.IncludeSelf, x.IsActive, x.Priority);
}

[ApiController, Route("api/user-visibility-policies")]
public sealed class UserVisibilityPoliciesController(MetivonDbContext db, IAccessControlAuditWriter audit) : ControllerBase
{
    [HttpPost("query"), PermissionAuthorize("access-control.user-visibility-assignments.view")]
    public async Task<IActionResult> Query(UserVisibilityPolicyQuery request, CancellationToken ct)
    {
        var query = db.Set<UserVisibilityPolicy>().AsNoTracking().Include(x => x.User).ThenInclude(x => x.Detail).Include(x => x.VisibilityPolicy).AsQueryable(); if (request.UserId.HasValue) query = query.Where(x => x.UserId == request.UserId);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); query = query.Where(x => x.User.Username.Contains(s) || x.User.Email.Contains(s) || x.VisibilityPolicy.Name.Contains(s) || x.VisibilityPolicy.EntityType.Contains(s)); }
        var total = await query.CountAsync(ct); var rows = await query.ApplyPagedSort(request, nameof(UserVisibilityPolicy.Id)).Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        return Ok(ApiResponse<PagedResult<UserVisibilityPolicyDto>>.Ok(new(rows.Select(Map).ToArray(), request.NormalizedPageNumber, request.NormalizedPageSize, total)));
    }
    [HttpGet("{id:long}"), PermissionAuthorize("access-control.user-visibility-assignments.view")]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => await Load(id, false, ct) is { } item ? Ok(ApiResponse<UserVisibilityPolicyDto>.Ok(Map(item))) : NotFound(ApiResponse<object>.Error("User visibility assignment was not found.", 404));
    [HttpPost, PermissionAuthorize("access-control.user-visibility-assignments.create")]
    public async Task<IActionResult> Create(CreateUserVisibilityPolicyRequest request, CancellationToken ct)
    {
        var error = await Validate(request.UserId, request.VisibilityPolicyId, null, ct); if (error is not null) return error; var entity = new UserVisibilityPolicy { UserId = request.UserId, VisibilityPolicyId = request.VisibilityPolicyId }; db.Add(entity); await audit.WriteAsync("Create", nameof(UserVisibilityPolicy), null, null, request, ct); await db.SaveChangesAsync(ct); return StatusCode(201, ApiResponse<UserVisibilityPolicyDto>.Ok(Map((await Load(entity.Id, false, ct))!), "User visibility assignment created."));
    }
    [HttpPut("{id:long}"), PermissionAuthorize("access-control.user-visibility-assignments.update")]
    public async Task<IActionResult> Update(long id, UpdateUserVisibilityPolicyRequest request, CancellationToken ct)
    {
        var entity = await db.Set<UserVisibilityPolicy>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("User visibility assignment was not found.", 404)); var userId = request.UserId ?? entity.UserId; var policyId = request.VisibilityPolicyId ?? entity.VisibilityPolicyId;
        var error = await Validate(userId, policyId, id, ct); if (error is not null) return error; var old = new { entity.UserId, entity.VisibilityPolicyId }; entity.UserId = userId; entity.VisibilityPolicyId = policyId; entity.UpdatedAt = DateTime.UtcNow; await audit.WriteAsync("Update", nameof(UserVisibilityPolicy), id.ToString(), old, request, ct); await db.SaveChangesAsync(ct); return Ok(ApiResponse<UserVisibilityPolicyDto>.Ok(Map((await Load(id, false, ct))!), "User visibility assignment updated."));
    }
    [HttpDelete("{id:long}"), PermissionAuthorize("access-control.user-visibility-assignments.delete")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct) { var entity = await db.Set<UserVisibilityPolicy>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("User visibility assignment was not found.", 404)); entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; await audit.WriteAsync("Delete", nameof(UserVisibilityPolicy), id.ToString(), entity, null, ct); await db.SaveChangesAsync(ct); return Ok(ApiResponse<object>.Ok(new { id }, "User visibility assignment deleted.")); }
    private Task<UserVisibilityPolicy?> Load(long id, bool tracking, CancellationToken ct) { IQueryable<UserVisibilityPolicy> q = db.Set<UserVisibilityPolicy>(); if (!tracking) q = q.AsNoTracking(); return q.Include(x => x.User).ThenInclude(x => x.Detail).Include(x => x.VisibilityPolicy).FirstOrDefaultAsync(x => x.Id == id, ct); }
    private async Task<IActionResult?> Validate(long userId, long policyId, long? excludedId, CancellationToken ct) { if (!await db.Users.AnyAsync(x => x.Id == userId, ct)) return NotFound(ApiResponse<object>.Error("User was not found.", 404)); var policy = await db.Set<VisibilityPolicy>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == policyId && x.IsActive, ct); if (policy is null) return BadRequest(ApiResponse<object>.Error("Visibility policy is invalid.", 400)); if (await db.Set<UserVisibilityPolicy>().AnyAsync(x => x.Id != excludedId && x.UserId == userId && x.VisibilityPolicy.EntityType == policy.EntityType, ct)) return Conflict(ApiResponse<object>.Error("The user already has a visibility policy for this entity type.", 409)); return null; }
    private static UserVisibilityPolicyDto Map(UserVisibilityPolicy x) { var name = x.User.Detail is null ? x.User.Username : string.Join(' ', new[] { x.User.Detail.FirstName, x.User.Detail.LastName }.Where(v => !string.IsNullOrWhiteSpace(v))); if (string.IsNullOrWhiteSpace(name)) name = x.User.Username; return new(x.Id, x.CreatedAt, x.UpdatedAt, x.DeletedAt, x.IsDeleted, x.UserId, name, x.VisibilityPolicyId, x.VisibilityPolicy.Name, x.VisibilityPolicy.EntityType, (int)x.VisibilityPolicy.ScopeType); }
}
