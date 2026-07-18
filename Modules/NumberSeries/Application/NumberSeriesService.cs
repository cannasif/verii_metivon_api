using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.NumberSeries.Domain.Entities;

namespace verii_metivon_api.Modules.NumberSeries.Application;

public sealed class NumberSeriesQuery : PagedQuery
{
    public string? Module { get; init; }
    public string? Reference { get; init; }
    public long? BranchId { get; init; }
    public bool? IsActive { get; init; }
}

public sealed record NumberSeriesRow(long Id,string Code,string Name,string Module,string Reference,string ScopeType,long? BranchId,long? WarehouseId,string Format,string ResetPeriod,long StartingNumber,long MaximumNumber,bool IsGibCompliant,bool AllowManual,bool IsContinuous,int ReservationTimeoutMinutes,bool IsDefault,int Priority,bool IsActive);
public sealed record NumberSeriesDetails(long Id,string Code,string Name,string Module,string Reference,NumberSeriesScopeType ScopeType,long? BranchId,long? WarehouseId,string Format,NumberSeriesResetPeriod ResetPeriod,long StartingNumber,int IncrementBy,long MaximumNumber,bool IsGibCompliant,bool AllowManual,bool IsContinuous,int ReservationTimeoutMinutes,bool IsDefault,int Priority,DateOnly? ValidFrom,DateOnly? ValidTo,bool IsActive,IReadOnlyList<NumberSeriesAssignmentInput> Assignments);
public sealed record NumberSeriesAssignmentInput(long? Id,long? BranchId,long? WarehouseId,long? UserId,long? BusinessPartnerId,string? Channel,string? Scenario,int Priority,bool IsDefault,bool IsActive);
public sealed record SaveNumberSeriesRequest(string Code,string Name,string Module,string Reference,NumberSeriesScopeType ScopeType,long? BranchId,long? WarehouseId,string Format,NumberSeriesResetPeriod ResetPeriod,long StartingNumber,int IncrementBy,long MaximumNumber,bool IsGibCompliant,bool AllowManual,bool IsContinuous,int? ReservationTimeoutMinutes,bool IsDefault,int Priority,DateOnly? ValidFrom,DateOnly? ValidTo,bool IsActive,IReadOnlyList<NumberSeriesAssignmentInput>? Assignments);
public sealed record ReserveNumberRequest(string Module,string Reference,long? NumberSeriesId,long? BranchId,long? WarehouseId,long? UserId,long? BusinessPartnerId,string? Channel,string? Scenario,DateOnly DocumentDate,string DocumentType);
public sealed record ReservedNumber(long UsageId,long NumberSeriesId,string NumberSeriesCode,string DocumentNumber,string PeriodKey,long SequenceNumber,DateTime ReservationExpiresAt);
public sealed record CleanupNumberReservationsRequest(string? Module,DateTime? ExpiredBefore);
public sealed record CleanupNumberReservationsResult(int Examined,int Recycled,int Cancelled,DateTime ExpiredBefore);
public sealed class NumberSeriesUsageQuery : PagedQuery
{
    public string? Module { get; init; }
    public string? Reference { get; init; }
    public string? DocumentType { get; init; }
    public NumberSeriesUsageStatus? Status { get; init; }
}
public sealed record NumberSeriesUsageRow(long Id,long NumberSeriesId,string SeriesCode,string SeriesName,string Module,string Reference,string PeriodKey,long SequenceNumber,string DocumentNumber,string Status,string DocumentType,long? DocumentId,long? UserId,DateTime ReservedAt,DateTime ReservationExpiresAt,DateTime? UsedAt,DateTime? CancelledAt,DateTime? RecycledAt,string? CancellationReason);

public interface INumberSeriesService
{
    Task<ApiResponse<PagedResult<NumberSeriesRow>>> GetPagedAsync(NumberSeriesQuery query,CancellationToken ct);
    Task<ApiResponse<NumberSeriesDetails>> GetByIdAsync(long id,CancellationToken ct);
    Task<ApiResponse<PagedResult<NumberSeriesUsageRow>>> GetUsagePagedAsync(NumberSeriesUsageQuery query,CancellationToken ct);
    Task<ApiResponse<object>> SaveAsync(long? id,SaveNumberSeriesRequest request,CancellationToken ct);
    Task<ApiResponse<object>> DeleteAsync(long id,CancellationToken ct);
    Task<ReservedNumber> ReserveAsync(ReserveNumberRequest request,CancellationToken ct);
    Task MarkUsedAsync(long usageId,long documentId,CancellationToken ct);
    Task<ApiResponse<object>> CancelAsync(long usageId,string reason,CancellationToken ct);
    Task<ApiResponse<CleanupNumberReservationsResult>> CleanupExpiredAsync(CleanupNumberReservationsRequest request,CancellationToken ct);
    Task<IReadOnlyList<NumberSeriesRow>> GetAvailableAsync(string module,string reference,long? branchId,long? warehouseId,CancellationToken ct);
}

public sealed class NumberSeriesService(MetivonDbContext db) : INumberSeriesService
{
    private static readonly Regex NumberToken = new(@"\{NUMBER(?::(?<length>\d{1,2}))?\}",RegexOptions.IgnoreCase|RegexOptions.Compiled);
    private static readonly Regex GibCode = new("^[A-Z0-9]{3}$",RegexOptions.Compiled);
    private static readonly Regex GibNumber = new("^[A-Z0-9]{3}[0-9]{13}$",RegexOptions.Compiled);

    public async Task<ApiResponse<PagedResult<NumberSeriesRow>>> GetPagedAsync(NumberSeriesQuery q,CancellationToken ct)
    {
        var query=db.DocumentNumberSeries.AsNoTracking().AsQueryable();
        if(!string.IsNullOrWhiteSpace(q.Module))query=query.Where(x=>x.Module==q.Module);
        if(!string.IsNullOrWhiteSpace(q.Reference))query=query.Where(x=>x.Reference==q.Reference);
        if(q.BranchId.HasValue)query=query.Where(x=>x.BranchId==q.BranchId||x.BranchId==null);
        if(q.IsActive.HasValue)query=query.Where(x=>x.IsActive==q.IsActive);
        if(!string.IsNullOrWhiteSpace(q.Search)){var s=q.Search.Trim();query=query.Where(x=>x.Code.Contains(s)||x.Name.Contains(s)||x.Module.Contains(s)||x.Reference.Contains(s));}
        query=query.ApplyPagedFilters(q);
        var total=await query.CountAsync(ct);
        var items=await query.ApplyPagedSort(q,nameof(DocumentNumberSeries.Code)).Skip((q.NormalizedPageNumber-1)*q.NormalizedPageSize).Take(q.NormalizedPageSize)
            .Select(x=>new NumberSeriesRow(x.Id,x.Code,x.Name,x.Module,x.Reference,x.ScopeType.ToString(),x.BranchId,x.WarehouseId,x.Format,x.ResetPeriod.ToString(),x.StartingNumber,x.MaximumNumber,x.IsGibCompliant,x.AllowManual,x.IsContinuous,x.ReservationTimeoutMinutes,x.IsDefault,x.Priority,x.IsActive)).ToListAsync(ct);
        return ApiResponse<PagedResult<NumberSeriesRow>>.Ok(new(items,q.NormalizedPageNumber,q.NormalizedPageSize,total));
    }

    public async Task<ApiResponse<NumberSeriesDetails>> GetByIdAsync(long id,CancellationToken ct)
    {
        var item=await db.DocumentNumberSeries.AsNoTracking().Where(x=>x.Id==id).Select(x=>new NumberSeriesDetails(
            x.Id,x.Code,x.Name,x.Module,x.Reference,x.ScopeType,x.BranchId,x.WarehouseId,x.Format,x.ResetPeriod,
            x.StartingNumber,x.IncrementBy,x.MaximumNumber,x.IsGibCompliant,x.AllowManual,x.IsContinuous,
            x.ReservationTimeoutMinutes,x.IsDefault,x.Priority,x.ValidFrom,x.ValidTo,x.IsActive,
            x.Assignments.OrderBy(a=>a.Priority).Select(a=>new NumberSeriesAssignmentInput(a.Id,a.BranchId,a.WarehouseId,a.UserId,a.BusinessPartnerId,a.Channel,a.Scenario,a.Priority,a.IsDefault,a.IsActive)).ToList()
        )).FirstOrDefaultAsync(ct);
        return item is null?ApiResponse<NumberSeriesDetails>.Error("Number series was not found.",404):ApiResponse<NumberSeriesDetails>.Ok(item);
    }

    public async Task<ApiResponse<PagedResult<NumberSeriesUsageRow>>> GetUsagePagedAsync(NumberSeriesUsageQuery q,CancellationToken ct)
    {
        var query=db.NumberSeriesUsages.AsNoTracking().Include(x=>x.NumberSeries).AsQueryable();
        if(!string.IsNullOrWhiteSpace(q.Module))query=query.Where(x=>x.NumberSeries.Module==q.Module);
        if(!string.IsNullOrWhiteSpace(q.Reference))query=query.Where(x=>x.NumberSeries.Reference==q.Reference);
        if(!string.IsNullOrWhiteSpace(q.DocumentType))query=query.Where(x=>x.DocumentType==q.DocumentType);
        if(q.Status.HasValue)query=query.Where(x=>x.Status==q.Status);
        if(!string.IsNullOrWhiteSpace(q.Search)){var s=q.Search.Trim();query=query.Where(x=>x.DocumentNumber.Contains(s)||x.NumberSeries.Code.Contains(s)||x.NumberSeries.Name.Contains(s)||x.DocumentType.Contains(s)||(x.CancellationReason!=null&&x.CancellationReason.Contains(s)));}
        query=query.ApplyPagedFilters(q);
        var total=await query.CountAsync(ct);
        var items=await query.ApplyPagedSort(q,nameof(NumberSeriesUsage.ReservedAt),true).Skip((q.NormalizedPageNumber-1)*q.NormalizedPageSize).Take(q.NormalizedPageSize)
            .Select(x=>new NumberSeriesUsageRow(x.Id,x.NumberSeriesId,x.NumberSeries.Code,x.NumberSeries.Name,x.NumberSeries.Module,x.NumberSeries.Reference,x.PeriodKey,x.SequenceNumber,x.DocumentNumber,x.Status.ToString(),x.DocumentType,x.DocumentId,x.UserId,x.ReservedAt,x.ReservationExpiresAt,x.UsedAt,x.CancelledAt,x.RecycledAt,x.CancellationReason)).ToListAsync(ct);
        return ApiResponse<PagedResult<NumberSeriesUsageRow>>.Ok(new(items,q.NormalizedPageNumber,q.NormalizedPageSize,total));
    }

    public async Task<ApiResponse<object>> SaveAsync(long? id,SaveNumberSeriesRequest r,CancellationToken ct)
    {
        var error=Validate(r);if(error is not null)return ApiResponse<object>.Error(error,400);
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        var code=r.Code.Trim().ToUpperInvariant();
        if(await db.DocumentNumberSeries.AnyAsync(x=>x.Code==code&&x.Id!=id,ct))return ApiResponse<object>.Error("Number series code already exists.",409);
        var entity=id.HasValue?await db.DocumentNumberSeries.Include(x=>x.Assignments).FirstOrDefaultAsync(x=>x.Id==id,ct):null;
        if(id.HasValue&&entity is null)return ApiResponse<object>.Error("Number series was not found.",404);
        if(entity is null){entity=new DocumentNumberSeries();db.Add(entity);}
        entity.Code=code;entity.Name=r.Name.Trim();entity.Module=r.Module.Trim();entity.Reference=r.Reference.Trim();entity.ScopeType=r.ScopeType;entity.BranchId=r.BranchId;entity.WarehouseId=r.WarehouseId;entity.Format=r.Format.Trim().ToUpperInvariant();entity.ResetPeriod=r.ResetPeriod;entity.StartingNumber=r.StartingNumber;entity.IncrementBy=r.IncrementBy;entity.MaximumNumber=r.MaximumNumber;entity.IsGibCompliant=r.IsGibCompliant;entity.AllowManual=r.AllowManual;entity.IsContinuous=r.IsContinuous;entity.ReservationTimeoutMinutes=r.ReservationTimeoutMinutes??30;entity.IsDefault=r.IsDefault;entity.Priority=r.Priority;entity.ValidFrom=r.ValidFrom;entity.ValidTo=r.ValidTo;entity.IsActive=r.IsActive;
        if(r.IsDefault){var defaults=await db.DocumentNumberSeries.Where(x=>x.Id!=entity.Id&&x.Module==entity.Module&&x.Reference==entity.Reference&&x.BranchId==entity.BranchId&&x.WarehouseId==entity.WarehouseId&&x.IsDefault).ToListAsync(ct);defaults.ForEach(x=>x.IsDefault=false);}
        if(id.HasValue){foreach(var old in entity.Assignments.Where(x=>r.Assignments?.All(a=>a.Id!=x.Id)!=false))xDelete(old);}
        foreach(var input in r.Assignments??[]){var assignment=input.Id.HasValue?entity.Assignments.FirstOrDefault(x=>x.Id==input.Id):null;if(assignment is null){assignment=new NumberSeriesAssignment{NumberSeries=entity};entity.Assignments.Add(assignment);}assignment.BranchId=input.BranchId;assignment.WarehouseId=input.WarehouseId;assignment.UserId=input.UserId;assignment.BusinessPartnerId=input.BusinessPartnerId;assignment.Channel=Clean(input.Channel);assignment.Scenario=Clean(input.Scenario);assignment.Priority=input.Priority;assignment.IsDefault=input.IsDefault;assignment.IsActive=input.IsActive;}
        await db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
        return ApiResponse<object>.Ok(new{entity.Id,entity.Code});
        void xDelete(NumberSeriesAssignment x){x.IsDeleted=true;x.DeletedAt=DateTime.UtcNow;}
    }

    public async Task<ApiResponse<object>> DeleteAsync(long id,CancellationToken ct)
    {
        var entity=await db.DocumentNumberSeries.FirstOrDefaultAsync(x=>x.Id==id,ct);if(entity is null)return ApiResponse<object>.Error("Number series was not found.",404);
        if(await db.NumberSeriesUsages.AnyAsync(x=>x.NumberSeriesId==id,ct)){entity.IsActive=false;await db.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{entity.Id,Deactivated=true},"Used number series was deactivated to preserve audit history.");}
        entity.IsDeleted=true;entity.DeletedAt=DateTime.UtcNow;await db.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{entity.Id});
    }

    public async Task<IReadOnlyList<NumberSeriesRow>> GetAvailableAsync(string module,string reference,long? branchId,long? warehouseId,CancellationToken ct)
    {
        var today=DateOnly.FromDateTime(DateTime.UtcNow);
        return await db.DocumentNumberSeries.AsNoTracking().Where(x=>x.IsActive&&x.Module==module&&x.Reference==reference&&(x.BranchId==null||x.BranchId==branchId)&&(x.WarehouseId==null||x.WarehouseId==warehouseId)&&(!x.ValidFrom.HasValue||x.ValidFrom<=today)&&(!x.ValidTo.HasValue||x.ValidTo>=today)).OrderByDescending(x=>x.IsDefault).ThenBy(x=>x.Priority).ThenBy(x=>x.Code).Select(x=>new NumberSeriesRow(x.Id,x.Code,x.Name,x.Module,x.Reference,x.ScopeType.ToString(),x.BranchId,x.WarehouseId,x.Format,x.ResetPeriod.ToString(),x.StartingNumber,x.MaximumNumber,x.IsGibCompliant,x.AllowManual,x.IsContinuous,x.ReservationTimeoutMinutes,x.IsDefault,x.Priority,x.IsActive)).ToListAsync(ct);
    }

    public async Task<ReservedNumber> ReserveAsync(ReserveNumberRequest r,CancellationToken ct)
    {
        var series=await ResolveAsync(r,ct)??throw new InvalidOperationException($"No active number series is assigned to {r.Module}/{r.Reference}.");
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        var period=PeriodKey(series.ResetPeriod,r.DocumentDate);
        var counter=await db.NumberSeriesCounters.FirstOrDefaultAsync(x=>x.NumberSeriesId==series.Id&&x.PeriodKey==period,ct);
        if(counter is null){counter=new NumberSeriesCounter{NumberSeriesId=series.Id,PeriodKey=period,NextNumber=series.StartingNumber};db.Add(counter);await db.SaveChangesAsync(ct);}
        if(counter.NextNumber>series.MaximumNumber)throw new InvalidOperationException($"Number series {series.Code} is exhausted for period {period}.");
        var sequence=counter.NextNumber;counter.NextNumber+=series.IncrementBy;
        var number=Format(series,r.DocumentDate,sequence);
        if(series.IsGibCompliant&&!GibNumber.IsMatch(number))throw new InvalidOperationException("Generated GIB document number must contain a 3-character alphanumeric series, 4-digit year and 9-digit sequential number.");
        var now=DateTime.UtcNow;var usage=new NumberSeriesUsage{NumberSeriesId=series.Id,NumberSeriesCounterId=counter.Id,PeriodKey=period,SequenceNumber=sequence,DocumentNumber=number,DocumentType=r.DocumentType,UserId=r.UserId,ReservedAt=now,ReservationExpiresAt=now.AddMinutes(series.ReservationTimeoutMinutes)};
        db.Add(usage);await db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
        return new ReservedNumber(usage.Id,series.Id,series.Code,number,period,sequence,usage.ReservationExpiresAt);
    }

    public async Task MarkUsedAsync(long usageId,long documentId,CancellationToken ct){var usage=await db.NumberSeriesUsages.FirstAsync(x=>x.Id==usageId,ct);if(usage.Status!=NumberSeriesUsageStatus.Reserved)throw new InvalidOperationException($"Document number {usage.DocumentNumber} is no longer reserved.");usage.Status=NumberSeriesUsageStatus.Used;usage.DocumentId=documentId;usage.UsedAt=DateTime.UtcNow;await db.SaveChangesAsync(ct);}
    public async Task<ApiResponse<object>> CancelAsync(long usageId,string reason,CancellationToken ct){if(string.IsNullOrWhiteSpace(reason))return ApiResponse<object>.Error("Cancellation reason is required.",400);var usage=await db.NumberSeriesUsages.FirstOrDefaultAsync(x=>x.Id==usageId,ct);if(usage is null)return ApiResponse<object>.Error("Number usage was not found.",404);if(usage.Status==NumberSeriesUsageStatus.Used)return ApiResponse<object>.Error("A used document number cannot be cancelled; void the business document instead.",409);if(usage.Status!=NumberSeriesUsageStatus.Reserved)return ApiResponse<object>.Error("Number usage is already closed.",409);usage.Status=NumberSeriesUsageStatus.Cancelled;usage.CancelledAt=DateTime.UtcNow;usage.CancellationReason=reason.Trim();await db.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{usage.Id,usage.DocumentNumber,usage.Status});}

    public async Task<ApiResponse<CleanupNumberReservationsResult>> CleanupExpiredAsync(CleanupNumberReservationsRequest request,CancellationToken ct)
    {
        var cutoff=request.ExpiredBefore?.ToUniversalTime()??DateTime.UtcNow;
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        var query=db.NumberSeriesUsages.Include(x=>x.NumberSeries).Where(x=>x.Status==NumberSeriesUsageStatus.Reserved&&x.ReservationExpiresAt<=cutoff);
        if(!string.IsNullOrWhiteSpace(request.Module))query=query.Where(x=>x.NumberSeries.Module==request.Module);
        var expired=await query.OrderByDescending(x=>x.SequenceNumber).Take(1000).ToListAsync(ct);var recycled=0;var cancelled=0;
        foreach(var usage in expired)
        {
            var counter=await db.NumberSeriesCounters.FirstAsync(x=>x.Id==usage.NumberSeriesCounterId,ct);
            var canRecycle=usage.NumberSeries.IsContinuous&&counter.NextNumber==usage.SequenceNumber+usage.NumberSeries.IncrementBy;
            usage.Status=NumberSeriesUsageStatus.Cancelled;usage.CancelledAt=cutoff;
            if(canRecycle){counter.NextNumber=usage.SequenceNumber;usage.RecycledAt=cutoff;usage.CancellationReason="AUTO_RECYCLED_EXPIRED_RESERVATION";recycled++;}
            else{usage.CancellationReason="AUTO_CANCELLED_EXPIRED_RESERVATION";cancelled++;}
        }
        await db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
        return ApiResponse<CleanupNumberReservationsResult>.Ok(new(expired.Count,recycled,cancelled,cutoff));
    }

    private async Task<DocumentNumberSeries?> ResolveAsync(ReserveNumberRequest r,CancellationToken ct)
    {
        if(r.NumberSeriesId.HasValue)return await db.DocumentNumberSeries.FirstOrDefaultAsync(x=>x.Id==r.NumberSeriesId&&x.IsActive&&x.Module==r.Module&&x.Reference==r.Reference,ct);
        var date=r.DocumentDate;
        var candidates=await db.DocumentNumberSeries.AsNoTracking().Include(x=>x.Assignments).Where(x=>x.IsActive&&x.Module==r.Module&&x.Reference==r.Reference&&(x.BranchId==null||x.BranchId==r.BranchId)&&(x.WarehouseId==null||x.WarehouseId==r.WarehouseId)&&(!x.ValidFrom.HasValue||x.ValidFrom<=date)&&(!x.ValidTo.HasValue||x.ValidTo>=date)).ToListAsync(ct);
        return candidates.Select(x=>new{Series=x,Assignment=x.Assignments.Where(a=>a.IsActive&&(!a.BranchId.HasValue||a.BranchId==r.BranchId)&&(!a.WarehouseId.HasValue||a.WarehouseId==r.WarehouseId)&&(!a.UserId.HasValue||a.UserId==r.UserId)&&(!a.BusinessPartnerId.HasValue||a.BusinessPartnerId==r.BusinessPartnerId)&&(a.Channel==null||a.Channel==r.Channel)&&(a.Scenario==null||a.Scenario==r.Scenario)).OrderBy(a=>a.Priority).FirstOrDefault()}).Where(x=>x.Assignment is not null||x.Series.IsDefault).OrderByDescending(x=>x.Assignment is not null).ThenBy(x=>x.Assignment?.Priority??x.Series.Priority).ThenByDescending(x=>x.Series.WarehouseId.HasValue).ThenByDescending(x=>x.Series.BranchId.HasValue).ThenByDescending(x=>x.Series.IsDefault).Select(x=>x.Series).FirstOrDefault();
    }

    private static string? Validate(SaveNumberSeriesRequest r)
    {
        if(string.IsNullOrWhiteSpace(r.Code)||string.IsNullOrWhiteSpace(r.Name)||string.IsNullOrWhiteSpace(r.Module)||string.IsNullOrWhiteSpace(r.Reference))return "Code, name, module and reference are required.";
        if(!NumberToken.IsMatch(r.Format))return "Format must contain a NUMBER token.";
        if(r.StartingNumber<0||r.IncrementBy<=0||r.MaximumNumber<r.StartingNumber)return "Starting, increment and maximum number values are invalid.";
        if((r.ReservationTimeoutMinutes??30) is <1 or >1440)return "Reservation timeout must be between 1 and 1440 minutes.";
        if(r.ValidFrom.HasValue&&r.ValidTo<r.ValidFrom)return "Valid-to date cannot be earlier than valid-from date.";
        if(r.ScopeType==NumberSeriesScopeType.Branch&&!r.BranchId.HasValue)return "Branch scope requires a branch.";
        if(r.ScopeType==NumberSeriesScopeType.Warehouse&&!r.WarehouseId.HasValue)return "Warehouse scope requires a warehouse.";
        if(r.IsGibCompliant&&(!GibCode.IsMatch(r.Code.Trim().ToUpperInvariant())||r.ResetPeriod!=NumberSeriesResetPeriod.Yearly||!string.Equals(r.Format.Trim(),"{SERIES}{YYYY}{NUMBER:9}",StringComparison.OrdinalIgnoreCase)))return "GIB series requires a 3-character alphanumeric code, yearly reset and {SERIES}{YYYY}{NUMBER:9} format.";
        return null;
    }
    private static string Format(DocumentNumberSeries s,DateOnly date,long number){var result=s.Format.Replace("{SERIES}",s.Code,StringComparison.OrdinalIgnoreCase).Replace("{YYYY}",date.ToString("yyyy"),StringComparison.OrdinalIgnoreCase).Replace("{YY}",date.ToString("yy"),StringComparison.OrdinalIgnoreCase).Replace("{MM}",date.ToString("MM"),StringComparison.OrdinalIgnoreCase).Replace("{DD}",date.ToString("dd"),StringComparison.OrdinalIgnoreCase);return NumberToken.Replace(result,m=>number.ToString().PadLeft(Math.Clamp(int.TryParse(m.Groups["length"].Value,out var length)?length:1,1,18),'0'));}
    private static string PeriodKey(NumberSeriesResetPeriod reset,DateOnly date)=>reset switch{NumberSeriesResetPeriod.Yearly=>date.ToString("yyyy"),NumberSeriesResetPeriod.Monthly=>date.ToString("yyyyMM"),NumberSeriesResetPeriod.Daily=>date.ToString("yyyyMMdd"),_=>"ALL"};
    private static string? Clean(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim().ToUpperInvariant();
}
