using verii_metivon_api.Core.Domain;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Warehouses.Domain.Entities;
namespace verii_metivon_api.Modules.Transfers.Domain.Entities;
public enum TransferOrderStatus{Draft=0,Confirmed=1,Shipped=2,PartiallyReceived=3,Received=4,Cancelled=5}
public sealed class TransferOrder:Entity{public string TransferNumber{get;set;}=string.Empty;public TransferOrderStatus Status{get;set;}public long FromWarehouseId{get;set;}public Warehouse FromWarehouse{get;set;}=null!;public long ToWarehouseId{get;set;}public Warehouse ToWarehouse{get;set;}=null!;public DateOnly TransferDate{get;set;}public DateOnly? ExpectedReceiptDate{get;set;}public DateTime? ShippedAt{get;set;}public DateTime? ReceivedAt{get;set;}public Guid? IssuePostingId{get;set;}public Guid? ReceiptPostingId{get;set;}public string? Notes{get;set;}public ICollection<TransferOrderLine>Lines{get;set;}=new List<TransferOrderLine>();}
public sealed class TransferOrderLine:Entity{public long TransferOrderId{get;set;}public TransferOrder TransferOrder{get;set;}=null!;public int LineNumber{get;set;}public long ProductId{get;set;}public Product Product{get;set;}=null!;public long UnitId{get;set;}public long FromLocationId{get;set;}public StorageLocation FromLocation{get;set;}=null!;public long ToLocationId{get;set;}public StorageLocation ToLocation{get;set;}=null!;public long InventoryStatusId{get;set;}public long? InventoryLotId{get;set;}public long? InventorySerialId{get;set;}public decimal RequestedQuantity{get;set;}public decimal ShippedQuantity{get;set;}public decimal ReceivedQuantity{get;set;}public string? Notes{get;set;}}
public sealed class TransferParameterSettings:Entity
{
 public long? BranchId{get;set;}public Branch? Branch{get;set;}public long? WarehouseId{get;set;}public Warehouse? Warehouse{get;set;}
 public bool AllowCrossBranchTransfer{get;set;}public bool RequireConfirmationBeforeShipment{get;set;}=true;public bool AutoConfirmOnCreate{get;set;}
 public bool RequireExpectedReceiptDate{get;set;}=true;public int DefaultTransitDays{get;set;}=1;public bool AllowPastTransferDate{get;set;}
 public bool AllowReceiptBeforeExpectedDate{get;set;}=true;public bool RequireNotes{get;set;}public int MaximumLinesPerTransfer{get;set;}=100;
 public bool RequireLotForLotTracked{get;set;}=true;public bool RequireSerialForSerialTracked{get;set;}=true;public long? DefaultInventoryStatusId{get;set;}
 public string InventoryCurrencyCode{get;set;}="TRY";public byte[] RowVersion{get;set;}=[];
}
