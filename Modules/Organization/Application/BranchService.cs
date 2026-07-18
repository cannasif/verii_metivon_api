using System.Data;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;

namespace verii_metivon_api.Modules.Organization.Application;

public sealed class BranchQuery : PagedQuery { public bool? IsActive { get; init; } }
public sealed record BranchRow(long Id,string Code,string Name,bool IsDefault,bool IsActive,DateTime CreatedAt,DateTime? UpdatedAt);
public sealed record SaveBranchRequest(string Code,string Name,bool IsDefault,bool IsActive);

public interface IBranchService
{
    Task<IReadOnlyList<BranchItem>> GetActiveAsync(CancellationToken ct);
    Task<ApiResponse<PagedResult<BranchRow>>> GetPagedAsync(BranchQuery query,CancellationToken ct);
    Task<ApiResponse<BranchRow>> GetByIdAsync(long id,CancellationToken ct);
    Task<ApiResponse<BranchRow>> SaveAsync(long? id,SaveBranchRequest request,CancellationToken ct);
    Task<ApiResponse<object>> DeleteAsync(long id,CancellationToken ct);
}

public sealed class BranchService(MetivonDbContext db) : IBranchService
{
    public async Task<IReadOnlyList<BranchItem>> GetActiveAsync(CancellationToken ct)=>await db.Branches.AsNoTracking().Where(x=>x.IsActive)
        .OrderByDescending(x=>x.IsDefault).ThenBy(x=>x.Name).Select(x=>new BranchItem(x.Id,x.Code,x.Name,x.IsDefault)).ToListAsync(ct);

    public async Task<ApiResponse<PagedResult<BranchRow>>> GetPagedAsync(BranchQuery q,CancellationToken ct)
    {
        var query=db.Branches.AsNoTracking().AsQueryable();
        if(q.IsActive.HasValue)query=query.Where(x=>x.IsActive==q.IsActive);
        if(!string.IsNullOrWhiteSpace(q.Search)){var search=q.Search.Trim();query=query.Where(x=>x.Code.Contains(search)||x.Name.Contains(search));}
        query=query.ApplyPagedFilters(q);
        var total=await query.CountAsync(ct);
        var items=await query.ApplyPagedSort(q,nameof(Core.Domain.Branch.Code)).Skip((q.NormalizedPageNumber-1)*q.NormalizedPageSize).Take(q.NormalizedPageSize)
            .Select(x=>new BranchRow(x.Id,x.Code,x.Name,x.IsDefault,x.IsActive,x.CreatedAt,x.UpdatedAt)).ToListAsync(ct);
        return ApiResponse<PagedResult<BranchRow>>.Ok(new(items,q.NormalizedPageNumber,q.NormalizedPageSize,total));
    }

    public async Task<ApiResponse<BranchRow>> GetByIdAsync(long id,CancellationToken ct)
    {
        var row=await db.Branches.AsNoTracking().Where(x=>x.Id==id).Select(x=>new BranchRow(x.Id,x.Code,x.Name,x.IsDefault,x.IsActive,x.CreatedAt,x.UpdatedAt)).FirstOrDefaultAsync(ct);
        return row is null?ApiResponse<BranchRow>.Error("Branch was not found.",404):ApiResponse<BranchRow>.Ok(row);
    }

    public async Task<ApiResponse<BranchRow>> SaveAsync(long? id,SaveBranchRequest request,CancellationToken ct)
    {
        var code=request.Code?.Trim().ToUpperInvariant()??string.Empty;var name=request.Name?.Trim()??string.Empty;
        if(code.Length is <1 or >30)return ApiResponse<BranchRow>.Error("Branch code must contain between 1 and 30 characters.",400);
        if(name.Length is <2 or >150)return ApiResponse<BranchRow>.Error("Branch name must contain between 2 and 150 characters.",400);
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        if(await db.Branches.AnyAsync(x=>x.Code==code&&x.Id!=id,ct))return ApiResponse<BranchRow>.Error("Branch code already exists.",409);
        var entity=id.HasValue?await db.Branches.FirstOrDefaultAsync(x=>x.Id==id,ct):null;
        if(id.HasValue&&entity is null)return ApiResponse<BranchRow>.Error("Branch was not found.",404);
        if(entity is null){entity=new Core.Domain.Branch();db.Branches.Add(entity);}
        if(entity.IsDefault&&!request.IsDefault&&await db.Branches.CountAsync(x=>x.IsDefault&&x.IsActive,ct)==1)
            return ApiResponse<BranchRow>.Error("At least one active default branch is required.",409);
        entity.Code=code;entity.Name=name;entity.IsDefault=request.IsDefault;entity.IsActive=request.IsActive;entity.UpdatedAt=id.HasValue?DateTime.UtcNow:null;
        if(request.IsDefault)
        {
            var otherDefaults=await db.Branches.Where(x=>x.Id!=entity.Id&&x.IsDefault).ToListAsync(ct);
            foreach(var branch in otherDefaults){branch.IsDefault=false;branch.UpdatedAt=DateTime.UtcNow;}
        }
        await db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
        return ApiResponse<BranchRow>.Ok(new(entity.Id,entity.Code,entity.Name,entity.IsDefault,entity.IsActive,entity.CreatedAt,entity.UpdatedAt));
    }

    public async Task<ApiResponse<object>> DeleteAsync(long id,CancellationToken ct)
    {
        var entity=await db.Branches.FirstOrDefaultAsync(x=>x.Id==id,ct);if(entity is null)return ApiResponse<object>.Error("Branch was not found.",404);
        if(entity.IsDefault)return ApiResponse<object>.Error("The default branch cannot be deleted. Assign another default branch first.",409);
        if(await db.Users.AnyAsync(x=>x.BranchId==id,ct)||await db.Warehouses.AnyAsync(x=>x.BranchId==id,ct))
            return ApiResponse<object>.Error("The branch is in use and cannot be deleted. Deactivate it instead.",409);
        entity.IsDeleted=true;entity.IsActive=false;entity.DeletedAt=DateTime.UtcNow;await db.SaveChangesAsync(ct);
        return ApiResponse<object>.Ok(new{entity.Id});
    }
}
