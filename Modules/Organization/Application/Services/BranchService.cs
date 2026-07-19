using System.Data;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Localization;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Organization.Application.Abstractions;
using verii_metivon_api.Modules.Organization.Application.Dtos;
using verii_metivon_api.Modules.Organization.Application.Queries;
using verii_metivon_api.Modules.Organization.Localization;

namespace verii_metivon_api.Modules.Organization.Application.Services;

public sealed class BranchService(
    MetivonDbContext db,
    IValidator<SaveBranchRequest> validator,
    IMapper mapper,
    IModuleLocalizer<OrganizationLocalizationResource> localizer,
    TimeProvider timeProvider) : IBranchService
{
    public async Task<IReadOnlyList<BranchOptionDto>> GetActiveAsync(CancellationToken cancellationToken) =>
        await db.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Name)
            .Select(x => new BranchOptionDto(x.Id, x.Code, x.Name, x.IsDefault))
            .ToListAsync(cancellationToken);

    public async Task<ApiResponse<PagedResult<BranchRow>>> GetPagedAsync(
        BranchQuery request,
        CancellationToken cancellationToken)
    {
        var query = db.Branches.AsNoTracking().AsQueryable();
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
        }

        query = query.ApplyPagedFilters(request);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .ApplyPagedSort(request, nameof(Branch.Code))
            .Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize)
            .Take(request.NormalizedPageSize)
            .Select(x => new BranchRow(x.Id, x.Code, x.Name, x.IsDefault, x.IsActive, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<BranchRow>>.Ok(
            new PagedResult<BranchRow>(items, request.NormalizedPageNumber, request.NormalizedPageSize, totalCount));
    }

    public async Task<ApiResponse<BranchRow>> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var row = await db.Branches
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new BranchRow(x.Id, x.Code, x.Name, x.IsDefault, x.IsActive, x.CreatedAt, x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? Error<BranchRow>("Organization.Branch.NotFound", 404)
            : ApiResponse<BranchRow>.Ok(row);
    }

    public async Task<ApiResponse<BranchRow>> SaveAsync(
        long? id,
        SaveBranchRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errorCode = validation.Errors[0].ErrorCode;
            return Error<BranchRow>(errorCode, 400);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        var name = request.Name.Trim();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        if (await db.Branches.AnyAsync(x => x.Code == code && x.Id != id, cancellationToken))
            return Error<BranchRow>("Organization.Branch.CodeExists", 409);

        var entity = id.HasValue
            ? await db.Branches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            : null;
        if (id.HasValue && entity is null) return Error<BranchRow>("Organization.Branch.NotFound", 404);

        entity ??= new Branch();
        if (!id.HasValue) db.Branches.Add(entity);

        if (entity.IsDefault && !request.IsDefault &&
            await db.Branches.CountAsync(x => x.IsDefault && x.IsActive, cancellationToken) == 1)
            return Error<BranchRow>("Organization.Branch.DefaultRequired", 409);

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        entity.Code = code;
        entity.Name = name;
        entity.IsDefault = request.IsDefault;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = id.HasValue ? utcNow : null;

        if (request.IsDefault)
        {
            var otherDefaults = await db.Branches.Where(x => x.Id != entity.Id && x.IsDefault).ToListAsync(cancellationToken);
            foreach (var branch in otherDefaults)
            {
                branch.IsDefault = false;
                branch.UpdatedAt = utcNow;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ApiResponse<BranchRow>.Ok(mapper.Map<BranchRow>(entity));
    }

    public async Task<ApiResponse<object>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await db.Branches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Error<object>("Organization.Branch.NotFound", 404);
        if (entity.IsDefault) return Error<object>("Organization.Branch.DefaultDeleteForbidden", 409);
        if (await db.Users.AnyAsync(x => x.BranchId == id, cancellationToken) ||
            await db.Warehouses.AnyAsync(x => x.BranchId == id, cancellationToken))
            return Error<object>("Organization.Branch.InUse", 409);

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAt = utcNow;
        await db.SaveChangesAsync(cancellationToken);
        return ApiResponse<object>.Ok(new { entity.Id });
    }

    private ApiResponse<T> Error<T>(string code, int statusCode) =>
        ApiResponse<T>.Error(localizer[code], statusCode, code);
}

