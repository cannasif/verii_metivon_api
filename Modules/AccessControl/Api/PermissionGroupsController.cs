using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Authorization;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;
using verii_metivon_api.Modules.AccessControl.Infrastructure.Persistence;

namespace verii_metivon_api.Modules.AccessControl.Api;

[ApiController, Route("api/permission-groups")]
public sealed class PermissionGroupsController(MetivonDbContext db, IAccessControlAuditWriter audit, IConfiguration configuration) : ControllerBase
{
    [HttpPost("restore-defaults"), PermissionAuthorize("access-control.permission-groups.update")]
    public async Task<IActionResult> RestoreDefaults(CancellationToken ct)
    {
        await DefaultPermissionGroupSeeder.ApplyAsync(db, configuration["BootstrapAdmin:Email"], ct);
        await audit.WriteAsync("RestoreDefaults", nameof(PermissionGroup), null, null, new { source = "BuiltInPresetCatalog" }, ct);
        await db.SaveChangesAsync(ct);

        var groups = await db.Set<PermissionGroup>().AsNoTracking()
            .Include(x => x.GroupPermissions).ThenInclude(x => x.PermissionDefinition)
            .Where(x => x.IsSystem)
            .OrderBy(x => x.Priority)
            .ToListAsync(ct);

        return Ok(ApiResponse<IReadOnlyList<PermissionGroupDto>>.Ok(
            groups.Select(Map).ToArray(),
            "Built-in permission groups restored."));
    }

    [HttpPost("query"), PermissionAuthorize("access-control.permission-groups.view")]
    public async Task<IActionResult> Query(PermissionGroupQuery request, CancellationToken ct)
    {
        var query = db.Set<PermissionGroup>().AsNoTracking().Include(x => x.GroupPermissions).ThenInclude(x => x.PermissionDefinition).AsQueryable();
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); query = query.Where(x => x.Code.Contains(s) || x.Name.Contains(s) || (x.Description != null && x.Description.Contains(s))); }
        query = query.ApplyPagedFilters(request); var total = await query.CountAsync(ct);
        var rows = await query.ApplyPagedSort(request, nameof(PermissionGroup.Name)).Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        return Ok(ApiResponse<PagedResult<PermissionGroupDto>>.Ok(new(rows.Select(Map).ToArray(), request.NormalizedPageNumber, request.NormalizedPageSize, total)));
    }

    [HttpGet("{id:long}"), PermissionAuthorize("access-control.permission-groups.view")]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => await Load(id, false, ct) is { } item
        ? Ok(ApiResponse<PermissionGroupDto>.Ok(Map(item))) : NotFound(ApiResponse<object>.Error("Permission group was not found.", 404));

    [HttpPost, PermissionAuthorize("access-control.permission-groups.create")]
    public async Task<IActionResult> Create(CreatePermissionGroupRequest request, CancellationToken ct)
    {
        if (request.IsSystemAdmin) return Conflict(ApiResponse<object>.Error("System administrator groups can only be provisioned by the system seed.", 409, "BUILT_IN_ADMIN_GROUP_REQUIRED"));
        var code = NormalizeCode(request.Code ?? request.Name); if (await db.Set<PermissionGroup>().AnyAsync(x => x.Code == code || x.Name == request.Name.Trim(), ct)) return Conflict(ApiResponse<object>.Error("Permission group code or name already exists.", 409));
        var validIds = await ValidateIds(request.PermissionDefinitionIds, ct); if (validIds.Count != request.PermissionDefinitionIds.Distinct().Count()) return BadRequest(ApiResponse<object>.Error("One or more permission definitions are invalid.", 400));
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var entity = new PermissionGroup { Code = code, Name = request.Name.Trim(), Description = request.Description?.Trim(), IsSystemAdmin = request.IsSystemAdmin, IsActive = request.IsActive, Priority = request.Priority };
        db.Add(entity); await db.SaveChangesAsync(ct);
        db.AddRange(validIds.Select(id => new PermissionGroupPermission { PermissionGroupId = entity.Id, PermissionDefinitionId = id }));
        await audit.WriteAsync("Create", nameof(PermissionGroup), entity.Id.ToString(), null, request, ct); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return StatusCode(201, ApiResponse<PermissionGroupDto>.Ok(Map((await Load(entity.Id, false, ct))!), "Permission group created."));
    }

    [HttpPut("{id:long}"), PermissionAuthorize("access-control.permission-groups.update")]
    public async Task<IActionResult> Update(long id, UpdatePermissionGroupRequest request, CancellationToken ct)
    {
        var entity = await db.Set<PermissionGroup>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("Permission group was not found.", 404));
        if (entity.IsSystem) return Conflict(ApiResponse<object>.Error("Built-in permission groups cannot be modified. Create a custom group instead.", 409, "BUILT_IN_GROUP_IMMUTABLE"));
        if (request.IsSystemAdmin.HasValue && request.IsSystemAdmin.Value != entity.IsSystemAdmin) return Conflict(ApiResponse<object>.Error("System administrator status cannot be changed through CRUD operations.", 409, "ADMIN_ESCALATION_BLOCKED"));
        var old = new { entity.Code, entity.Name, entity.Description, entity.IsSystemAdmin, entity.IsActive, entity.Priority };
        if (request.Code is not null && !entity.IsSystem) entity.Code = NormalizeCode(request.Code); if (request.Name is not null) entity.Name = request.Name.Trim(); if (request.Description is not null) entity.Description = request.Description.Trim();
        if (request.IsSystemAdmin.HasValue) entity.IsSystemAdmin = request.IsSystemAdmin.Value; if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value; if (request.Priority.HasValue) entity.Priority = request.Priority.Value;
        entity.UpdatedAt = DateTime.UtcNow; await audit.WriteAsync("Update", nameof(PermissionGroup), id.ToString(), old, request, ct); await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<PermissionGroupDto>.Ok(Map((await Load(id, false, ct))!), "Permission group updated."));
    }

    [HttpPut("{id:long}/permissions"), PermissionAuthorize("access-control.permission-groups.update")]
    public async Task<IActionResult> SetPermissions(long id, SetPermissionGroupPermissionsRequest request, CancellationToken ct)
    {
        var group = await db.Set<PermissionGroup>().FirstOrDefaultAsync(x => x.Id == id, ct); if (group is null) return NotFound(ApiResponse<object>.Error("Permission group was not found.", 404));
        if (group.IsSystem) return Conflict(ApiResponse<object>.Error("Built-in permission groups cannot be modified. Create a custom group instead.", 409, "BUILT_IN_GROUP_IMMUTABLE"));
        var ids = await ValidateIds(request.PermissionDefinitionIds, ct); if (ids.Count != request.PermissionDefinitionIds.Distinct().Count()) return BadRequest(ApiResponse<object>.Error("One or more permission definitions are invalid.", 400));
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var links = await db.Set<PermissionGroupPermission>().Where(x => x.PermissionGroupId == id).ToListAsync(ct); var oldIds = links.Select(x => x.PermissionDefinitionId).ToArray(); db.RemoveRange(links);
        db.AddRange(ids.Select(permissionId => new PermissionGroupPermission { PermissionGroupId = id, PermissionDefinitionId = permissionId }));
        await audit.WriteAsync("SetPermissions", nameof(PermissionGroup), id.ToString(), oldIds, ids, ct); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return Ok(ApiResponse<object>.Ok(new { id, permissionDefinitionIds = ids }, "Group permissions updated."));
    }

    [HttpDelete("{id:long}"), PermissionAuthorize("access-control.permission-groups.delete")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await db.Set<PermissionGroup>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("Permission group was not found.", 404));
        if (entity.IsSystem) return Conflict(ApiResponse<object>.Error("System permission groups cannot be deleted.", 409));
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; await audit.WriteAsync("Delete", nameof(PermissionGroup), id.ToString(), entity, null, ct); await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(new { id }, "Permission group deleted."));
    }

    private Task<PermissionGroup?> Load(long id, bool tracking, CancellationToken ct) { IQueryable<PermissionGroup> q = db.Set<PermissionGroup>(); if (!tracking) q = q.AsNoTracking(); return q.Include(x => x.GroupPermissions).ThenInclude(x => x.PermissionDefinition).FirstOrDefaultAsync(x => x.Id == id, ct); }
    private async Task<List<long>> ValidateIds(IEnumerable<long> requested, CancellationToken ct) { var ids = requested.Distinct().ToArray(); return await db.Set<PermissionDefinition>().Where(x => ids.Contains(x.Id) && x.IsActive).Select(x => x.Id).ToListAsync(ct); }
    private static string NormalizeCode(string value) => value.Trim().ToLowerInvariant().Replace(' ', '-');
    private static PermissionGroupDto Map(PermissionGroup x) => new(x.Id, x.CreatedAt, x.UpdatedAt, x.DeletedAt, x.IsDeleted, x.Name, x.Description, x.IsSystemAdmin, x.IsActive,
        x.GroupPermissions.Where(y => !y.IsDenied && y.PermissionDefinition.IsActive).Select(y => y.PermissionDefinitionId).Distinct().ToArray(),
        x.GroupPermissions.Where(y => !y.IsDenied && y.PermissionDefinition.IsActive).Select(y => y.PermissionDefinition.Code).Distinct().ToArray(), x.Code, x.Priority, x.IsSystem);
}

[ApiController, Route("api/user-permission-groups")]
public sealed class UserPermissionGroupsController(MetivonDbContext db, IAccessControlAuditWriter audit) : ControllerBase
{
    [HttpGet("{userId:long}"), PermissionAuthorize("access-control.user-group-assignments.view")]
    public async Task<IActionResult> Get(long userId, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(x => x.Id == userId, ct)) return NotFound(ApiResponse<object>.Error("User was not found.", 404));
        var links = await db.Set<UserPermissionGroup>().AsNoTracking().Where(x => x.UserId == userId).Include(x => x.PermissionGroup).OrderBy(x => x.PermissionGroup.Priority).ToListAsync(ct);
        return Ok(ApiResponse<UserPermissionGroupDto>.Ok(new(userId, links.Select(x => x.PermissionGroupId).ToArray(), links.Select(x => x.PermissionGroup.Name).ToArray())));
    }

    [HttpPut("{userId:long}"), PermissionAuthorize("access-control.user-group-assignments.update")]
    public async Task<IActionResult> Set(long userId, SetUserPermissionGroupsRequest request, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(x => x.Id == userId, ct)) return NotFound(ApiResponse<object>.Error("User was not found.", 404));
        var ids = request.PermissionGroupIds.Distinct().ToArray(); var valid = await db.Set<PermissionGroup>().Where(x => ids.Contains(x.Id) && x.IsActive).Select(x => x.Id).ToListAsync(ct);
        if (valid.Count != ids.Length) return BadRequest(ApiResponse<object>.Error("One or more permission groups are invalid.", 400));
        await using var tx = await db.Database.BeginTransactionAsync(ct); var old = await db.Set<UserPermissionGroup>().Where(x => x.UserId == userId).ToListAsync(ct); db.RemoveRange(old);
        db.AddRange(valid.Select(groupId => new UserPermissionGroup { UserId = userId, PermissionGroupId = groupId })); await audit.WriteAsync("SetUserGroups", nameof(UserPermissionGroup), userId.ToString(), old.Select(x => x.PermissionGroupId), valid, ct);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); return Ok(ApiResponse<object>.Ok(new { userId, permissionGroupIds = valid }, "User permission groups updated."));
    }
}
