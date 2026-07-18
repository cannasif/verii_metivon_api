using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Inventory.Domain.Entities;

namespace verii_metivon_api.Modules.Traceability.Application.Parameters;

public sealed record InventoryTraceabilityParametersDto(long? BranchId,long? WarehouseId,bool RequireLotForLotTrackedProducts,bool RequireSerialForSerialTrackedProducts,bool RequireExpiryDateForShelfLifeProducts,bool RequireManufactureDateWhenExpiryExists,bool RejectExpiredReceipts,int MinimumRemainingShelfLifeDays,decimal MinimumRemainingShelfLifePercent,bool PreventDuplicateSerialNumbers,bool BlockExpiredShipments,bool EnforceFefoOnShipments,int ExpiryWarningDays,bool AllowMixedLotsPerDocument,bool AutoCreateLabelsAfterReceipt,int DefaultLabelCopies);
public sealed record SaveInventoryTraceabilityParametersRequest(long? BranchId,long? WarehouseId,bool RequireLotForLotTrackedProducts,bool RequireSerialForSerialTrackedProducts,bool RequireExpiryDateForShelfLifeProducts,bool RequireManufactureDateWhenExpiryExists,bool RejectExpiredReceipts,int MinimumRemainingShelfLifeDays,decimal MinimumRemainingShelfLifePercent,bool PreventDuplicateSerialNumbers,bool BlockExpiredShipments,bool EnforceFefoOnShipments,int ExpiryWarningDays,bool AllowMixedLotsPerDocument,bool AutoCreateLabelsAfterReceipt,int DefaultLabelCopies);

public interface IInventoryTraceabilityParameterService
{
    Task<InventoryTraceabilityParametersDto> GetAsync(long? branchId,long? warehouseId,CancellationToken ct);
    Task<InventoryTraceabilityParametersDto> SaveAsync(SaveInventoryTraceabilityParametersRequest request,CancellationToken ct);
    Task<InventoryTraceabilityParameterSettings> ResolveSettingsAsync(long? branchId,long? warehouseId,CancellationToken ct);
}

public sealed class InventoryTraceabilityParameterService(MetivonDbContext db):IInventoryTraceabilityParameterService
{
    public async Task<InventoryTraceabilityParametersDto> GetAsync(long? branchId,long? warehouseId,CancellationToken ct)=>Map(await ResolveSettingsAsync(branchId,warehouseId,ct),branchId,warehouseId);

    public async Task<InventoryTraceabilityParametersDto> SaveAsync(SaveInventoryTraceabilityParametersRequest r,CancellationToken ct)
    {
        Validate(r);
        if(r.BranchId.HasValue&&!await db.Branches.AnyAsync(x=>x.Id==r.BranchId&&x.IsActive,ct))throw new ArgumentException("Selected branch is not active.");
        if(r.WarehouseId.HasValue&&!await db.Warehouses.AnyAsync(x=>x.Id==r.WarehouseId&&x.IsActive&&(!r.BranchId.HasValue||x.BranchId==r.BranchId),ct))throw new ArgumentException("Selected warehouse is not active or does not belong to the selected branch.");
        var p=await db.InventoryTraceabilityParameterSettings.FirstOrDefaultAsync(x=>x.BranchId==r.BranchId&&x.WarehouseId==r.WarehouseId,ct);
        if(p is null){p=new InventoryTraceabilityParameterSettings{BranchId=r.BranchId,WarehouseId=r.WarehouseId};db.Add(p);}
        Apply(p,r);await db.SaveChangesAsync(ct);return Map(p,r.BranchId,r.WarehouseId);
    }

    public async Task<InventoryTraceabilityParameterSettings> ResolveSettingsAsync(long? branchId,long? warehouseId,CancellationToken ct)=>await db.InventoryTraceabilityParameterSettings.AsNoTracking().Where(x=>(x.BranchId==branchId||x.BranchId==null)&&(x.WarehouseId==warehouseId||x.WarehouseId==null)).OrderByDescending(x=>x.WarehouseId.HasValue).ThenByDescending(x=>x.BranchId.HasValue).FirstOrDefaultAsync(ct)??new InventoryTraceabilityParameterSettings{BranchId=branchId,WarehouseId=warehouseId};

    static InventoryTraceabilityParametersDto Map(InventoryTraceabilityParameterSettings p,long? branchId,long? warehouseId)=>new(branchId,warehouseId,p.RequireLotForLotTrackedProducts,p.RequireSerialForSerialTrackedProducts,p.RequireExpiryDateForShelfLifeProducts,p.RequireManufactureDateWhenExpiryExists,p.RejectExpiredReceipts,p.MinimumRemainingShelfLifeDays,p.MinimumRemainingShelfLifePercent,p.PreventDuplicateSerialNumbers,p.BlockExpiredShipments,p.EnforceFefoOnShipments,p.ExpiryWarningDays,p.AllowMixedLotsPerDocument,p.AutoCreateLabelsAfterReceipt,p.DefaultLabelCopies);
    static void Apply(InventoryTraceabilityParameterSettings p,SaveInventoryTraceabilityParametersRequest r){p.RequireLotForLotTrackedProducts=r.RequireLotForLotTrackedProducts;p.RequireSerialForSerialTrackedProducts=r.RequireSerialForSerialTrackedProducts;p.RequireExpiryDateForShelfLifeProducts=r.RequireExpiryDateForShelfLifeProducts;p.RequireManufactureDateWhenExpiryExists=r.RequireManufactureDateWhenExpiryExists;p.RejectExpiredReceipts=r.RejectExpiredReceipts;p.MinimumRemainingShelfLifeDays=r.MinimumRemainingShelfLifeDays;p.MinimumRemainingShelfLifePercent=r.MinimumRemainingShelfLifePercent;p.PreventDuplicateSerialNumbers=r.PreventDuplicateSerialNumbers;p.BlockExpiredShipments=r.BlockExpiredShipments;p.EnforceFefoOnShipments=r.EnforceFefoOnShipments;p.ExpiryWarningDays=r.ExpiryWarningDays;p.AllowMixedLotsPerDocument=r.AllowMixedLotsPerDocument;p.AutoCreateLabelsAfterReceipt=r.AutoCreateLabelsAfterReceipt;p.DefaultLabelCopies=r.DefaultLabelCopies;}
    static void Validate(SaveInventoryTraceabilityParametersRequest r){if(r.MinimumRemainingShelfLifeDays<0||r.MinimumRemainingShelfLifePercent is <0 or >100||r.ExpiryWarningDays<0||r.DefaultLabelCopies is <1 or >100)throw new ArgumentException("One or more traceability parameter values are invalid.");}
}
