namespace verii_metivon_api.Modules.Procurement.Domain.Enums;
public enum PurchaseOrderStatus{Draft=0,PendingApproval=1,Approved=2,Confirmed=3,PartiallyReceived=4,Received=5,PartiallyInvoiced=6,Invoiced=7,Cancelled=8,Closed=9}
public enum PurchaseOrderType{Standard=1,Return=2,Blanket=3,DropShipment=4}
public enum PurchaseLineStatus{Open=0,PartiallyReceived=1,Received=2,Cancelled=3,Closed=4}
