using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Authorization;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;
using verii_metivon_api.Modules.AccessControl.Infrastructure.Persistence;

namespace verii_metivon_api.Modules.AccessControl.Api;

[ApiController, Route("api/permission-definitions")]
public sealed class PermissionDefinitionsController(MetivonDbContext db, IAccessControlAuditWriter audit) : ControllerBase
{
    [HttpPost("query"), PermissionAuthorize("access-control.permission-definitions.view")]
    public async Task<IActionResult> Query(PermissionDefinitionQuery request, CancellationToken ct)
    {
        var query = db.Set<PermissionDefinition>().AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); query = query.Where(x => x.Code.Contains(s) || x.Name.Contains(s) || (x.Description != null && x.Description.Contains(s))); }
        query = query.ApplyPagedFilters(request);
        var total = await query.CountAsync(ct);
        var rows = await query.ApplyPagedSort(request, nameof(PermissionDefinition.Code)).Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        return Ok(ApiResponse<PagedResult<PermissionDefinitionDto>>.Ok(new(rows.Select(Map).ToArray(), request.NormalizedPageNumber, request.NormalizedPageSize, total)));
    }

    [HttpGet("{id:long}"), PermissionAuthorize("access-control.permission-definitions.view")]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => await db.Set<PermissionDefinition>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct) is { } item
        ? Ok(ApiResponse<PermissionDefinitionDto>.Ok(Map(item))) : NotFound(ApiResponse<object>.Error("Permission definition was not found.", 404));

    [HttpPost, PermissionAuthorize("access-control.permission-definitions.create")]
    public async Task<IActionResult> Create(SavePermissionDefinitionRequest request, CancellationToken ct)
    {
        var code = NormalizeCode(request.Code); if (string.IsNullOrWhiteSpace(code)) return BadRequest(ApiResponse<object>.Error("Permission code is required.", 400));
        if (await db.Set<PermissionDefinition>().AnyAsync(x => x.Code == code, ct)) return Conflict(ApiResponse<object>.Error("Permission code already exists.", 409));
        var entity = New(request, code); db.Add(entity); await audit.WriteAsync("Create", nameof(PermissionDefinition), null, null, entity, ct); await db.SaveChangesAsync(ct);
        return StatusCode(201, ApiResponse<PermissionDefinitionDto>.Ok(Map(entity), "Permission definition created."));
    }

    [HttpPut("{id:long}"), PermissionAuthorize("access-control.permission-definitions.update")]
    public async Task<IActionResult> Update(long id, UpdatePermissionDefinitionRequest request, CancellationToken ct)
    {
        var entity = await db.Set<PermissionDefinition>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("Permission definition was not found.", 404));
        var old = Map(entity); if (request.Code is not null) entity.Code = NormalizeCode(request.Code); if (request.Name is not null) entity.Name = request.Name.Trim();
        if (request.Description is not null) entity.Description = request.Description.Trim(); if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        if (request.AvailableOnWeb.HasValue) entity.AvailableOnWeb = request.AvailableOnWeb.Value; if (request.AvailableOnMobile.HasValue) entity.AvailableOnMobile = request.AvailableOnMobile.Value;
        ParseCode(entity); entity.UpdatedAt = DateTime.UtcNow; await audit.WriteAsync("Update", nameof(PermissionDefinition), id.ToString(), old, entity, ct); await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<PermissionDefinitionDto>.Ok(Map(entity), "Permission definition updated."));
    }

    [HttpDelete("{id:long}"), PermissionAuthorize("access-control.permission-definitions.delete")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var entity = await db.Set<PermissionDefinition>().FirstOrDefaultAsync(x => x.Id == id, ct); if (entity is null) return NotFound(ApiResponse<object>.Error("Permission definition was not found.", 404));
        if (entity.IsSystem) return Conflict(ApiResponse<object>.Error("System permission definitions cannot be deleted.", 409));
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; await audit.WriteAsync("Delete", nameof(PermissionDefinition), id.ToString(), entity, null, ct); await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<object>.Ok(new { id }, "Permission definition deleted."));
    }

    [HttpPost("sync"), PermissionAuthorize("access-control.permission-definitions.update")]
    public async Task<IActionResult> Sync(SyncPermissionDefinitionsRequest request, CancellationToken ct)
    {
        var created = 0; var updated = 0; var reactivated = 0;
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var normalizedItems = request.Items.Where(x => !string.IsNullOrWhiteSpace(x.Code))
            .GroupBy(x => NormalizeCode(x.Code)).Select(x => x.First()).ToArray();
        var requestedCodes = normalizedItems.Select(x => NormalizeCode(x.Code)).ToArray();
        var existingByCode = await db.Set<PermissionDefinition>().IgnoreQueryFilters()
            .Where(x => requestedCodes.Contains(x.Code)).ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase, ct);
        foreach (var item in normalizedItems)
        {
            var code = NormalizeCode(item.Code); existingByCode.TryGetValue(code, out var entity);
            if (entity is null) { entity = New(new(item.Code, item.Name ?? code, item.Description, item.IsActive, item.AvailableOnWeb, item.AvailableOnMobile), code); entity.IsSystem = true; db.Add(entity); created++; continue; }
            var changed = false; if (entity.IsDeleted && request.ReactivateSoftDeleted) { entity.IsDeleted = false; entity.DeletedAt = null; reactivated++; changed = true; }
            if (request.UpdateExistingNames && !string.IsNullOrWhiteSpace(item.Name) && entity.Name != item.Name) { entity.Name = item.Name.Trim(); changed = true; }
            if (request.UpdateExistingDescriptions && entity.Description != item.Description) { entity.Description = item.Description; changed = true; }
            if (request.UpdateExistingIsActive && entity.IsActive != item.IsActive) { entity.IsActive = item.IsActive; changed = true; }
            if (entity.AvailableOnWeb != item.AvailableOnWeb || entity.AvailableOnMobile != item.AvailableOnMobile) { entity.AvailableOnWeb = item.AvailableOnWeb; entity.AvailableOnMobile = item.AvailableOnMobile; changed = true; }
            entity.IsSystem = true; ParseCode(entity); if (changed) { entity.UpdatedAt = DateTime.UtcNow; updated++; }
        }
        await audit.WriteAsync("Sync", nameof(PermissionDefinition), null, null, new { created, updated, reactivated, count = request.Items.Count }, ct); await db.SaveChangesAsync(ct);
        await DefaultPermissionGroupSeeder.ApplyAsync(db, ct: ct);
        await tx.CommitAsync(ct);
        return Ok(ApiResponse<PermissionDefinitionSyncResultDto>.Ok(new(created, updated, reactivated, request.Items.Count), "Permission catalog synchronized."));
    }

    private static PermissionDefinition New(SavePermissionDefinitionRequest r, string code) { var e = new PermissionDefinition { Code = code, Name = r.Name.Trim(), Description = r.Description?.Trim(), IsActive = r.IsActive, AvailableOnWeb = r.AvailableOnWeb, AvailableOnMobile = r.AvailableOnMobile }; ParseCode(e); return e; }
    private static void ParseCode(PermissionDefinition e) { var parts = e.Code.Split('.', StringSplitOptions.RemoveEmptyEntries); e.Module = parts.ElementAtOrDefault(0) ?? "general"; e.Action = parts.LastOrDefault() ?? "view"; e.Resource = parts.Length > 2 ? string.Join('.', parts.Skip(1).Take(parts.Length - 2)) : parts.ElementAtOrDefault(1) ?? "general"; }
    private static string NormalizeCode(string code) => code.Trim().ToLowerInvariant().Replace(' ', '-');
    private static PermissionDefinitionDto Map(PermissionDefinition x) => new(x.Id, x.CreatedAt, x.UpdatedAt, x.DeletedAt, x.IsDeleted, x.Code, x.Name, x.Description, x.IsActive, x.AvailableOnWeb, x.AvailableOnMobile, x.Module, x.Resource, x.Action);
}
