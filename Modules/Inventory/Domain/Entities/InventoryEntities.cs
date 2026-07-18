using verii_metivon_api.Core.Domain;using verii_metivon_api.Modules.Inventory.Domain.Enums;using verii_metivon_api.Modules.Products.Domain.Entities;using verii_metivon_api.Modules.Warehouses.Domain.Entities;
namespace verii_metivon_api.Modules.Inventory.Domain.Entities;
public sealed class InventoryStatus:DefinitionEntity{public bool IsAvailable{get;set;}=true;public bool IsReservable{get;set;}=true;public bool IsNettable{get;set;}=true;}
public sealed class InventoryLot:Entity{public long ProductId{get;set;}public Product Product{get;set;}=null!;public string LotNumber{get;set;}=string.Empty;public DateOnly? ManufactureDate{get;set;}public DateOnly? ExpiryDate{get;set;}public DateOnly? BestBeforeDate{get;set;}public string? SupplierLotNumber{get;set;}public bool IsBlocked{get;set;}public bool IsActive{get;set;}=true;}
public sealed class InventorySerial:Entity{public long ProductId{get;set;}public Product Product{get;set;}=null!;public string SerialNumber{get;set;}=string.Empty;public long? InventoryLotId{get;set;}public InventoryLot? InventoryLot{get;set;}public int Status{get;set;}public bool IsActive{get;set;}=true;}
public sealed class InventoryTransaction:Entity{public Guid PostingId{get;set;}public string IdempotencyKey{get;set;}=string.Empty;public string DocumentType{get;set;}=string.Empty;public long DocumentId{get;set;}public long? DocumentLineId{get;set;}public string DocumentNumber{get;set;}=string.Empty;public DateTime PostingDate{get;set;}public InventoryMovementType MovementType{get;set;}public InventoryMovementDirection Direction{get;set;}public long ProductId{get;set;}public Product Product{get;set;}=null!;public long UnitId{get;set;}public Unit Unit{get;set;}=null!;public long WarehouseId{get;set;}public Warehouse Warehouse{get;set;}=null!;public long StorageLocationId{get;set;}public StorageLocation StorageLocation{get;set;}=null!;public long InventoryStatusId{get;set;}public InventoryStatus InventoryStatus{get;set;}=null!;public long? InventoryLotId{get;set;}public InventoryLot? InventoryLot{get;set;}public long? InventorySerialId{get;set;}public InventorySerial? InventorySerial{get;set;}public decimal Quantity{get;set;}public decimal BaseQuantity{get;set;}public decimal UnitCost{get;set;}public decimal TotalCost{get;set;}public string CurrencyCode{get;set;}="TRY";public long? ReversalOfId{get;set;}public InventoryTransaction? ReversalOf{get;set;}public string? Explanation{get;set;}}
public sealed class InventoryBalance:Entity{public long ProductId{get;set;}public long WarehouseId{get;set;}public long StorageLocationId{get;set;}public long InventoryStatusId{get;set;}public long? InventoryLotId{get;set;}public long? InventorySerialId{get;set;}public decimal PhysicalQuantity{get;set;}public decimal ReservedQuantity{get;set;}public decimal AvailableQuantity{get;set;}public decimal InventoryValue{get;set;}public byte[] RowVersion{get;set;}=Array.Empty<byte>();}
public sealed class InventoryReservation:Entity{public Guid ReservationNumber{get;set;}=Guid.NewGuid();public string SourceType{get;set;}=string.Empty;public long SourceId{get;set;}public long? SourceLineId{get;set;}public long ProductId{get;set;}public long WarehouseId{get;set;}public long? StorageLocationId{get;set;}public long InventoryStatusId{get;set;}public long? InventoryLotId{get;set;}public long? InventorySerialId{get;set;}public decimal Quantity{get;set;}public decimal ConsumedQuantity{get;set;}public ReservationStatus Status{get;set;}public DateTime? ExpiresAt{get;set;}}
public sealed class InventoryCostLayer:Entity{public long ProductId{get;set;}public long WarehouseId{get;set;}public long ReceiptTransactionId{get;set;}public InventoryTransaction ReceiptTransaction{get;set;}=null!;public DateTime ReceiptDate{get;set;}public decimal OriginalQuantity{get;set;}public decimal RemainingQuantity{get;set;}public decimal UnitCost{get;set;}public string CurrencyCode{get;set;}="TRY";public CostLayerStatus Status{get;set;}}
public sealed class InventoryTraceabilityParameterSettings:Entity
{
    public long? BranchId{get;set;}public Branch? Branch{get;set;}
    public long? WarehouseId{get;set;}public Warehouse? Warehouse{get;set;}
    public bool RequireLotForLotTrackedProducts{get;set;}=true;
    public bool RequireSerialForSerialTrackedProducts{get;set;}=true;
    public bool RequireExpiryDateForShelfLifeProducts{get;set;}=true;
    public bool RequireManufactureDateWhenExpiryExists{get;set;}
    public bool RejectExpiredReceipts{get;set;}=true;
    public int MinimumRemainingShelfLifeDays{get;set;}=30;
    public decimal MinimumRemainingShelfLifePercent{get;set;}
    public bool PreventDuplicateSerialNumbers{get;set;}=true;
    public bool BlockExpiredShipments{get;set;}=true;
    public bool EnforceFefoOnShipments{get;set;}=true;
    public int ExpiryWarningDays{get;set;}=60;
    public bool AllowMixedLotsPerDocument{get;set;}=true;
    public bool AutoCreateLabelsAfterReceipt{get;set;}=true;
    public int DefaultLabelCopies{get;set;}=1;
    public byte[] RowVersion{get;set;}=[];
}
