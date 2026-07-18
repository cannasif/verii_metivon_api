using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.Accounting.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Enums;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Warehouses.Domain.Entities;

namespace verii_metivon_api.Modules.Accounting.Application.Definitions;

public sealed class FiscalPeriodQuery : PagedQuery { public bool? IsOpen { get; init; } }
public sealed class InventoryPostingProfileQuery : PagedQuery { public InventoryMovementType? MovementType { get; init; } public bool? IsActive { get; init; } }

public sealed record FiscalPeriodRow(long Id, string Code, string Name, DateOnly StartDate, DateOnly EndDate, bool IsOpen, bool IsInventoryClosed, bool IsGeneralLedgerClosed);
public sealed record SaveFiscalPeriodRequest(string Code, string Name, DateOnly StartDate, DateOnly EndDate, bool IsOpen, bool IsInventoryClosed, bool IsGeneralLedgerClosed);
public sealed record InventoryPostingProfileRow(long Id, string Code, string Name, string MovementType, long DebitAccountId, string DebitAccount, long CreditAccountId, string CreditAccount, long? ProductGroupId, string? ProductGroup, long? WarehouseId, string? Warehouse, int Priority, bool IsActive);
public sealed record SaveInventoryPostingProfileRequest(string Code, string Name, InventoryMovementType MovementType, long DebitAccountId, long CreditAccountId, long? ProductGroupId, long? WarehouseId, int Priority, bool IsActive);

public interface IAccountingDefinitionService
{
    Task<ApiResponse<PagedResult<FiscalPeriodRow>>> GetFiscalPeriodsAsync(FiscalPeriodQuery query, CancellationToken ct);
    Task<ApiResponse<FiscalPeriodRow>> GetFiscalPeriodAsync(long id, CancellationToken ct);
    Task<ApiResponse<object>> CreateFiscalPeriodAsync(SaveFiscalPeriodRequest request, CancellationToken ct);
    Task<ApiResponse<object>> UpdateFiscalPeriodAsync(long id, SaveFiscalPeriodRequest request, CancellationToken ct);
    Task<ApiResponse<object>> DeleteFiscalPeriodAsync(long id, CancellationToken ct);
    Task<ApiResponse<PagedResult<InventoryPostingProfileRow>>> GetPostingProfilesAsync(InventoryPostingProfileQuery query, CancellationToken ct);
    Task<ApiResponse<InventoryPostingProfileRow>> GetPostingProfileAsync(long id, CancellationToken ct);
    Task<ApiResponse<object>> CreatePostingProfileAsync(SaveInventoryPostingProfileRequest request, CancellationToken ct);
    Task<ApiResponse<object>> UpdatePostingProfileAsync(long id, SaveInventoryPostingProfileRequest request, CancellationToken ct);
    Task<ApiResponse<object>> DeletePostingProfileAsync(long id, CancellationToken ct);
}

public sealed class AccountingDefinitionService(IUnitOfWork unitOfWork) : IAccountingDefinitionService
{
    public async Task<ApiResponse<FiscalPeriodRow>> GetFiscalPeriodAsync(long id, CancellationToken ct)
    {
        var row = await unitOfWork.Repository<FiscalPeriod>().Query().Where(x => !x.IsDeleted && x.Id == id).Select(x => new FiscalPeriodRow(x.Id, x.Code, x.Name, x.StartDate, x.EndDate, x.IsOpen, x.IsInventoryClosed, x.IsGeneralLedgerClosed)).FirstOrDefaultAsync(ct);
        return row is null ? ApiResponse<FiscalPeriodRow>.Error("Fiscal period was not found.", 404) : ApiResponse<FiscalPeriodRow>.Ok(row);
    }
    public async Task<ApiResponse<PagedResult<FiscalPeriodRow>>> GetFiscalPeriodsAsync(FiscalPeriodQuery query, CancellationToken ct)
    {
        var source = unitOfWork.Repository<FiscalPeriod>().Query().Where(x => !x.IsDeleted);
        if (query.IsOpen.HasValue) source = source.Where(x => x.IsOpen == query.IsOpen);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
        }
        source = source.ApplyPagedFilters(query);
        var total = await source.CountAsync(ct);
        var ordered = source.ApplyPagedSort(query, nameof(FiscalPeriod.StartDate), true);
        var rows = await ordered.ThenByDescending(x => x.Id)
            .Skip((query.NormalizedPageNumber - 1) * query.NormalizedPageSize).Take(query.NormalizedPageSize)
            .Select(x => new FiscalPeriodRow(x.Id, x.Code, x.Name, x.StartDate, x.EndDate, x.IsOpen, x.IsInventoryClosed, x.IsGeneralLedgerClosed)).ToListAsync(ct);
        return ApiResponse<PagedResult<FiscalPeriodRow>>.Ok(new(rows, query.NormalizedPageNumber, query.NormalizedPageSize, total));
    }

    public Task<ApiResponse<object>> CreateFiscalPeriodAsync(SaveFiscalPeriodRequest request, CancellationToken ct) => SaveFiscalPeriodAsync(null, request, ct);
    public Task<ApiResponse<object>> UpdateFiscalPeriodAsync(long id, SaveFiscalPeriodRequest request, CancellationToken ct) => SaveFiscalPeriodAsync(id, request, ct);

    private async Task<ApiResponse<object>> SaveFiscalPeriodAsync(long? id, SaveFiscalPeriodRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name)) return ApiResponse<object>.Error("Period code and name are required.", 400);
        if (request.StartDate > request.EndDate) return ApiResponse<object>.Error("Period start date cannot be after end date.", 400);
        if (request.IsGeneralLedgerClosed && !request.IsInventoryClosed) return ApiResponse<object>.Error("Inventory must be closed before the general ledger can be closed.", 400);
        var periods = unitOfWork.Repository<FiscalPeriod>();
        if (await periods.ExistsAsync(x => !x.IsDeleted && x.Code == code && x.Id != id, ct)) return ApiResponse<object>.Error("Fiscal period code already exists.", 409);
        if (await periods.ExistsAsync(x => !x.IsDeleted && x.Id != id && x.StartDate <= request.EndDate && x.EndDate >= request.StartDate, ct)) return ApiResponse<object>.Error("Fiscal period overlaps an existing period.", 409);
        FiscalPeriod entity;
        if (id.HasValue)
        {
            entity = await periods.GetByIdForUpdateAsync(id.Value, ct) ?? null!;
            if (entity is null || entity.IsDeleted) return ApiResponse<object>.Error("Fiscal period was not found.", 404);
            if (await unitOfWork.Repository<JournalEntry>().ExistsAsync(x => x.FiscalPeriodId == id && (x.PostingDate < request.StartDate || x.PostingDate > request.EndDate), ct)) return ApiResponse<object>.Error("The date range excludes existing journal entries.", 409);
        }
        else { entity = new FiscalPeriod(); await periods.AddAsync(entity, ct); }
        entity.Code = code; entity.Name = request.Name.Trim(); entity.StartDate = request.StartDate; entity.EndDate = request.EndDate;
        entity.IsOpen = request.IsOpen; entity.IsInventoryClosed = request.IsInventoryClosed; entity.IsGeneralLedgerClosed = request.IsGeneralLedgerClosed;
        if (id.HasValue) periods.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);
        return ApiResponse<object>.Ok(new { entity.Id });
    }

    public async Task<ApiResponse<object>> DeleteFiscalPeriodAsync(long id, CancellationToken ct)
    {
        if (await unitOfWork.Repository<JournalEntry>().ExistsAsync(x => x.FiscalPeriodId == id, ct) || await unitOfWork.Repository<InventoryCostClose>().ExistsAsync(x => x.FiscalPeriodId == id, ct)) return ApiResponse<object>.Error("A fiscal period with accounting activity cannot be deleted.", 409);
        if (!await unitOfWork.Repository<FiscalPeriod>().SoftDeleteAsync(id, ct)) return ApiResponse<object>.Error("Fiscal period was not found.", 404);
        await unitOfWork.SaveChangesAsync(ct); return ApiResponse<object>.Ok(new { Id = id });
    }

    public async Task<ApiResponse<PagedResult<InventoryPostingProfileRow>>> GetPostingProfilesAsync(InventoryPostingProfileQuery query, CancellationToken ct)
    {
        var profiles = unitOfWork.Repository<InventoryPostingProfile>().Query().Where(x => !x.IsDeleted);
        if (query.MovementType.HasValue) profiles = profiles.Where(x => x.MovementType == query.MovementType);
        if (query.IsActive.HasValue) profiles = profiles.Where(x => x.IsActive == query.IsActive);
        if (!string.IsNullOrWhiteSpace(query.Search)) { var search = query.Search.Trim(); profiles = profiles.Where(x => x.Code.Contains(search) || x.Name.Contains(search)); }
        profiles = profiles.ApplyPagedFilters(query);
        var total = await profiles.CountAsync(ct);
        var page = await profiles.ApplyPagedSort(query, nameof(InventoryPostingProfile.Priority), true).ThenBy(x => x.Code)
            .Skip((query.NormalizedPageNumber - 1) * query.NormalizedPageSize).Take(query.NormalizedPageSize).ToListAsync(ct);
        var accountIds = page.SelectMany(x => new[] { x.DebitAccountId, x.CreditAccountId }).Distinct().ToArray();
        var groupIds = page.Where(x => x.ProductGroupId.HasValue).Select(x => x.ProductGroupId!.Value).Distinct().ToArray();
        var warehouseIds = page.Where(x => x.WarehouseId.HasValue).Select(x => x.WarehouseId!.Value).Distinct().ToArray();
        var accounts = await unitOfWork.Repository<LedgerAccount>().Query().Where(x => accountIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct);
        var groups = await unitOfWork.Repository<ProductGroup>().Query().Where(x => groupIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct);
        var warehouses = await unitOfWork.Repository<Warehouse>().Query().Where(x => warehouseIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct);
        var rows = page.Select(x => new InventoryPostingProfileRow(x.Id, x.Code, x.Name, x.MovementType.ToString(), x.DebitAccountId, accounts.TryGetValue(x.DebitAccountId, out var debit) ? $"{debit.Code} - {debit.Name}" : "-", x.CreditAccountId, accounts.TryGetValue(x.CreditAccountId, out var credit) ? $"{credit.Code} - {credit.Name}" : "-", x.ProductGroupId, x.ProductGroupId.HasValue && groups.TryGetValue(x.ProductGroupId.Value, out var group) ? $"{group.Code} - {group.Name}" : null, x.WarehouseId, x.WarehouseId.HasValue && warehouses.TryGetValue(x.WarehouseId.Value, out var warehouse) ? $"{warehouse.Code} - {warehouse.Name}" : null, x.Priority, x.IsActive)).ToList();
        return ApiResponse<PagedResult<InventoryPostingProfileRow>>.Ok(new(rows, query.NormalizedPageNumber, query.NormalizedPageSize, total));
    }

    public async Task<ApiResponse<InventoryPostingProfileRow>> GetPostingProfileAsync(long id, CancellationToken ct)
    {
        var entity = await unitOfWork.Repository<InventoryPostingProfile>().Query().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, ct);
        if (entity is null) return ApiResponse<InventoryPostingProfileRow>.Error("Posting profile was not found.", 404);
        var debit = await unitOfWork.Repository<LedgerAccount>().GetByIdAsync(entity.DebitAccountId, ct); var credit = await unitOfWork.Repository<LedgerAccount>().GetByIdAsync(entity.CreditAccountId, ct);
        var group = entity.ProductGroupId.HasValue ? await unitOfWork.Repository<ProductGroup>().GetByIdAsync(entity.ProductGroupId.Value, ct) : null;
        var warehouse = entity.WarehouseId.HasValue ? await unitOfWork.Repository<Warehouse>().GetByIdAsync(entity.WarehouseId.Value, ct) : null;
        return ApiResponse<InventoryPostingProfileRow>.Ok(new(entity.Id, entity.Code, entity.Name, entity.MovementType.ToString(), entity.DebitAccountId, debit is null ? "-" : $"{debit.Code} - {debit.Name}", entity.CreditAccountId, credit is null ? "-" : $"{credit.Code} - {credit.Name}", entity.ProductGroupId, group is null ? null : $"{group.Code} - {group.Name}", entity.WarehouseId, warehouse is null ? null : $"{warehouse.Code} - {warehouse.Name}", entity.Priority, entity.IsActive));
    }

    public Task<ApiResponse<object>> CreatePostingProfileAsync(SaveInventoryPostingProfileRequest request, CancellationToken ct) => SavePostingProfileAsync(null, request, ct);
    public Task<ApiResponse<object>> UpdatePostingProfileAsync(long id, SaveInventoryPostingProfileRequest request, CancellationToken ct) => SavePostingProfileAsync(id, request, ct);

    private async Task<ApiResponse<object>> SavePostingProfileAsync(long? id, SaveInventoryPostingProfileRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name)) return ApiResponse<object>.Error("Profile code and name are required.", 400);
        if (!Enum.IsDefined(request.MovementType)) return ApiResponse<object>.Error("Inventory movement type is invalid.", 400);
        if (request.DebitAccountId == request.CreditAccountId) return ApiResponse<object>.Error("Debit and credit accounts must be different.", 400);
        if (request.Priority is < 0 or > 9999) return ApiResponse<object>.Error("Priority must be between 0 and 9999.", 400);
        var validAccounts = await unitOfWork.Repository<LedgerAccount>().Query().CountAsync(x => (x.Id == request.DebitAccountId || x.Id == request.CreditAccountId) && x.IsActive && x.AllowPosting, ct);
        if (validAccounts != 2) return ApiResponse<object>.Error("Both accounts must be active posting accounts.", 400);
        if (request.ProductGroupId.HasValue && !await unitOfWork.Repository<ProductGroup>().ExistsAsync(x => x.Id == request.ProductGroupId && x.IsActive, ct)) return ApiResponse<object>.Error("Active product group was not found.", 400);
        if (request.WarehouseId.HasValue && !await unitOfWork.Repository<Warehouse>().ExistsAsync(x => x.Id == request.WarehouseId && x.IsActive, ct)) return ApiResponse<object>.Error("Active warehouse was not found.", 400);
        var repository = unitOfWork.Repository<InventoryPostingProfile>();
        if (await repository.ExistsAsync(x => !x.IsDeleted && x.Id != id && x.Code == code && x.MovementType == request.MovementType, ct)) return ApiResponse<object>.Error("Posting profile code already exists for this movement type.", 409);
        InventoryPostingProfile entity;
        if (id.HasValue) { entity = await repository.GetByIdForUpdateAsync(id.Value, ct) ?? null!; if (entity is null || entity.IsDeleted) return ApiResponse<object>.Error("Posting profile was not found.", 404); }
        else { entity = new InventoryPostingProfile(); await repository.AddAsync(entity, ct); }
        entity.Code = code; entity.Name = request.Name.Trim(); entity.MovementType = request.MovementType; entity.DebitAccountId = request.DebitAccountId; entity.CreditAccountId = request.CreditAccountId; entity.ProductGroupId = request.ProductGroupId; entity.WarehouseId = request.WarehouseId; entity.Priority = request.Priority; entity.IsActive = request.IsActive;
        if (id.HasValue) repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct); return ApiResponse<object>.Ok(new { entity.Id });
    }

    public async Task<ApiResponse<object>> DeletePostingProfileAsync(long id, CancellationToken ct)
    {
        if (!await unitOfWork.Repository<InventoryPostingProfile>().SoftDeleteAsync(id, ct)) return ApiResponse<object>.Error("Posting profile was not found.", 404);
        await unitOfWork.SaveChangesAsync(ct); return ApiResponse<object>.Ok(new { Id = id });
    }
}


