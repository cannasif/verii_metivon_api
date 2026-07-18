namespace verii_metivon_api.Modules.Inventory.Domain.Enums;
public enum InventoryMovementDirection{Receipt=1,Issue=-1}
public enum InventoryMovementType{Opening=10,PurchaseReceipt=20,PurchaseReturn=21,SalesShipment=30,SalesReturn=31,TransferIssue=40,TransferReceipt=41,CountGain=50,CountLoss=51,AdjustmentIn=60,AdjustmentOut=61,CustomsReleaseIssue=70,CustomsReleaseReceipt=71,ExportCustomsIssue=72}
public enum ReservationStatus{Active=0,Consumed=1,Released=2,Expired=3}
public enum CostLayerStatus{Open=0,PartiallyConsumed=1,Closed=2}
