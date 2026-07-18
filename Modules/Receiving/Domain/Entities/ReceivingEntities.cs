using verii_metivon_api.Core.Domain;using verii_metivon_api.Modules.Procurement.Domain.Entities;using verii_metivon_api.Modules.Products.Domain.Entities;using verii_metivon_api.Modules.Receiving.Domain.Enums;using verii_metivon_api.Modules.Warehouses.Domain.Entities;
namespace verii_metivon_api.Modules.Receiving.Domain.Entities;
public sealed class GoodsReceipt:Entity{public string ReceiptNumber{get;set;}=string.Empty;public GoodsReceiptType ReceiptType{get;set;}public GoodsReceiptStatus Status{get;set;}public long BranchId{get;set;}public long? SupplierId{get;set;}public long? PurchaseOrderId{get;set;}public PurchaseOrder? PurchaseOrder{get;set;}public long? TradeDossierId{get;set;}public long WarehouseId{get;set;}public Warehouse Warehouse{get;set;}=null!;public string? SupplierDeliveryNoteNumber{get;set;}public DateOnly ReceiptDate{get;set;}public DateTime? PostedAt{get;set;}public Guid? InventoryPostingId{get;set;}public string? Notes{get;set;}public ICollection<GoodsReceiptLine> Lines{get;set;}=new List<GoodsReceiptLine>();}
public sealed class GoodsReceiptLine:Entity{public long GoodsReceiptId{get;set;}public GoodsReceipt GoodsReceipt{get;set;}=null!;public int LineNumber{get;set;}public long? PurchaseOrderLineId{get;set;}public PurchaseOrderLine? PurchaseOrderLine{get;set;}public long ProductId{get;set;}public Product Product{get;set;}=null!;public long UnitId{get;set;}public Unit Unit{get;set;}=null!;public long StorageLocationId{get;set;}public StorageLocation StorageLocation{get;set;}=null!;public long InventoryStatusId{get;set;}public decimal ExpectedQuantity{get;set;}public decimal ReceivedQuantity{get;set;}public decimal AcceptedQuantity{get;set;}public decimal RejectedQuantity{get;set;}public decimal UnitCost{get;set;}public string? LotNumber{get;set;}public DateOnly? ManufactureDate{get;set;}public DateOnly? ExpiryDate{get;set;}public string? Notes{get;set;}public ICollection<GoodsReceiptSerial> Serials{get;set;}=new List<GoodsReceiptSerial>();}
public sealed class GoodsReceiptSerial:Entity{public long GoodsReceiptLineId{get;set;}public GoodsReceiptLine GoodsReceiptLine{get;set;}=null!;public string SerialNumber{get;set;}=string.Empty;}
public sealed class ReceivingParameterSettings:Entity
{
    public long? BranchId{get;set;}public Branch? Branch{get;set;}
    public long? WarehouseId{get;set;}public Warehouse? Warehouse{get;set;}
    public bool RequirePurchaseOrder{get;set;}=true;
    public bool AllowFreeReceipt{get;set;}=true;
    public bool AllowPartialReceipt{get;set;}=true;
    public decimal OverDeliveryTolerancePercent{get;set;}
    public decimal UnderDeliveryTolerancePercent{get;set;}
    public bool RequireSupplierDeliveryNoteNumber{get;set;}
    public bool RequireLotNumberForLotTracked{get;set;}=true;
    public bool RequireSerialsForSerialTracked{get;set;}=true;
    public bool RequireExpiryDateForTrackedItems{get;set;}=true;
    public int MinimumRemainingShelfLifeDays{get;set;}
    public bool RequireQualityInspection{get;set;}
    public bool AutoCreateLabels{get;set;}=true;
    public int DefaultLabelCopies{get;set;}=1;
    public string InventoryCurrencyCode{get;set;}="TRY";
    public byte[] RowVersion{get;set;}=[];
}
