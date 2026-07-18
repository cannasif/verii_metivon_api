using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Authorization;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Modules.AccessControl.Api;

[ApiController, Route("api/audit-logs")]
public sealed class AuditLogsController(MetivonDbContext db) : ControllerBase
{
    [HttpPost("query"), PermissionAuthorize("access-control.audit-logs.view")]
    public Task<IActionResult> Query(AuditLogQuery request, CancellationToken ct) => Execute(db.Set<AccessControlAuditLog>().AsNoTracking(), request, ct);

    [HttpPost("trace/{traceId}/query"), PermissionAuthorize("access-control.audit-logs.view")]
    public Task<IActionResult> Trace(string traceId, AuditLogQuery request, CancellationToken ct) => Execute(db.Set<AccessControlAuditLog>().AsNoTracking().Where(x => x.TraceId == traceId), request, ct);

    [HttpPost("entity/{entityType}/{entityId}/query"), PermissionAuthorize("access-control.audit-logs.view")]
    public Task<IActionResult> Entity(string entityType, string entityId, AuditLogQuery request, CancellationToken ct) => Execute(db.Set<AccessControlAuditLog>().AsNoTracking().Where(x => x.EntityType == entityType && x.EntityId == entityId), request, ct);

    [HttpGet("{id:long}"), PermissionAuthorize("access-control.audit-logs.view")]
    public async Task<IActionResult> Get(long id, CancellationToken ct) => await db.Set<AccessControlAuditLog>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct) is { } item ? Ok(ApiResponse<AuditLogDto>.Ok(Map(item))) : NotFound(ApiResponse<object>.Error("Audit log was not found.", 404));

    private static async Task<IActionResult> Execute(IQueryable<AccessControlAuditLog> query, AuditLogQuery request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); query = query.Where(x => x.TraceId.Contains(s) || x.ActionType.Contains(s) || (x.EntityType != null && x.EntityType.Contains(s)) || (x.PerformedByUserEmail != null && x.PerformedByUserEmail.Contains(s))); }
        query = query.ApplyPagedFilters(request); var total = await query.CountAsync(ct); var rows = await query.ApplyPagedSort(request, nameof(AccessControlAuditLog.CreatedAt), true).Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        return new OkObjectResult(ApiResponse<PagedResult<AuditLogDto>>.Ok(new(rows.Select(Map).ToArray(), request.NormalizedPageNumber, request.NormalizedPageSize, total)));
    }
    private static AuditLogDto Map(AccessControlAuditLog x) => new(x.Id, x.TraceId, x.ActionType, x.EntityType, x.EntityId, x.Result, x.Source, x.BranchCode, x.RequestPath, x.RequestMethod, x.Reason, x.FailureReason, x.PerformedByUserId, x.PerformedByUserEmail, x.OldValuesJson, x.NewValuesJson, x.ChangedFieldsJson, [], x.CreatedAt);
}
