using verii_metivon_api.Core.Domain;
using verii_metivon_api.Modules.Products.Domain.Entities;

namespace verii_metivon_api.Modules.TradeOperations.Domain.Entities;

public enum TradeDirection { Import = 1, Export = 2 }
public enum TradeOperationType { Definitive = 1, Temporary = 2, InwardProcessing = 3, Return = 4, ReExport = 5 }
public enum TradeDossierStatus { Draft = 0, Booked = 10, InTransit = 20, Arrived = 30, TemporaryStorage = 40, CustomsWaiting = 50, Inspection = 60, TaxWaiting = 70, ReleaseWaiting = 80, Released = 90, ExitAuthorized = 100, Completed = 110, Closed = 120, Cancelled = 900 }
public enum CustomsDeclarationType { Import = 1, Export = 2, BondedWarehouse = 3, Transit = 4, ReExport = 5 }
public enum CustomsDeclarationStatus { Draft = 0, Registered = 10, Inspection = 20, TaxCalculated = 30, TaxPaid = 40, ReleaseWaiting = 50, Released = 60, Closed = 70, Cancelled = 900 }
public enum TradeTransportMode { Road = 1, Sea = 2, Air = 3, Rail = 4, Multimodal = 5, Courier = 6 }
public enum CustomsInspectionChannel { Unknown = 0, Blue = 1, Green = 2, Yellow = 3, Red = 4 }
public enum TradeLinkType { PurchaseOrder = 1, SalesOrder = 2, GoodsReceipt = 3, Shipment = 4, DeliveryNote = 5, ElectronicDocument = 6, ImportDossier = 7 }
public enum TradeAttachmentType { CustomsDeclaration = 1, CommercialInvoice = 2, PackingList = 3, BillOfLading = 4, AirWaybill = 5, CertificateOfOrigin = 6, InsurancePolicy = 7, InspectionReport = 8, TaxReceipt = 9, PaymentReceipt = 10, Other = 99 }

public sealed class TradeDossier : Entity
{
    public string DossierNumber { get; set; } = string.Empty; public TradeDirection Direction { get; set; } public TradeOperationType OperationType { get; set; } public TradeDossierStatus Status { get; set; }
    public long BranchId { get; set; } public long BusinessPartnerId { get; set; } public BusinessPartner BusinessPartner { get; set; } = null!; public long? CustomsBrokerId { get; set; } public long? CarrierId { get; set; }
    public string CurrencyCode { get; set; } = "USD"; public string IncotermCode { get; set; } = "FOB"; public string? PaymentMethodCode { get; set; } public string? DeliveryMethodCode { get; set; }
    public string? CustomsOfficeCode { get; set; } public long? BondedWarehouseId { get; set; } public TradeTransportMode TransportMode { get; set; } = TradeTransportMode.Road; public string? MasterTransportDocumentNumber { get; set; } public string? HouseTransportDocumentNumber { get; set; } public string? ContainerNumber { get; set; } public string? VehicleOrVoyageNumber { get; set; } public string? PortOfLoadingCode { get; set; } public string? PortOfDischargeCode { get; set; } public string? CountryOfDispatchCode { get; set; } public DateOnly OpenDate { get; set; } public DateOnly? EstimatedArrivalDate { get; set; } public DateOnly? ActualArrivalDate { get; set; }
    public DateTime? ReleasedAt { get; set; } public DateTime? ClosedAt { get; set; } public string? Notes { get; set; } public byte[] RowVersion { get; set; } = [];
    public ICollection<CustomsDeclaration> Declarations { get; set; } = new List<CustomsDeclaration>(); public ICollection<TradeDossierStatusHistory> StatusHistory { get; set; } = new List<TradeDossierStatusHistory>(); public ICollection<TradeDocumentLink> Links { get; set; } = new List<TradeDocumentLink>();
}
public sealed class CustomsDeclaration : Entity
{
    public long TradeDossierId { get; set; } public TradeDossier TradeDossier { get; set; } = null!; public CustomsDeclarationType DeclarationType { get; set; } public CustomsDeclarationStatus Status { get; set; }
    public string DeclarationNumber { get; set; } = string.Empty; public DateOnly DeclarationDate { get; set; } public string RegimeCode { get; set; } = string.Empty; public string CustomsOfficeCode { get; set; } = string.Empty;
    public long? BondedWarehouseId { get; set; } public decimal CustomsExchangeRate { get; set; } = 1; public string? GuaranteeReference { get; set; } public string? RegistrationNumber { get; set; } public CustomsInspectionChannel InspectionChannel { get; set; } public decimal TotalTaxes { get; set; } public DateTime? RegisteredAt { get; set; } public DateTime? ReleasedAt { get; set; } public DateTime? ClosedAt { get; set; } public string? Notes { get; set; } public byte[] RowVersion { get; set; } = [];
    public ICollection<CustomsDeclarationLine> Lines { get; set; } = new List<CustomsDeclarationLine>(); public ICollection<CustomsDeclarationStatusHistory> StatusHistory { get; set; } = new List<CustomsDeclarationStatusHistory>();
}
public sealed class CustomsDeclarationLine : Entity
{
    public long CustomsDeclarationId { get; set; } public CustomsDeclaration CustomsDeclaration { get; set; } = null!; public int LineNumber { get; set; } public long ProductId { get; set; } public Product Product { get; set; } = null!;
    public string GtipCode { get; set; } = string.Empty; public string? CountryOfOriginCode { get; set; } public decimal Quantity { get; set; } public long UnitId { get; set; } public decimal NetWeight { get; set; } public decimal GrossWeight { get; set; } public int PackageCount { get; set; }
    public decimal CustomsValue { get; set; } public decimal CustomsDuty { get; set; } public decimal VatAmount { get; set; } public decimal ReleasedQuantity { get; set; }
}
public sealed class TradeDossierStatusHistory : Entity
{
    public long TradeDossierId { get; set; } public TradeDossier TradeDossier { get; set; } = null!; public TradeDossierStatus FromStatus { get; set; } public TradeDossierStatus ToStatus { get; set; }
    public string? ReasonCode { get; set; } public DateTime OccurredAt { get; set; } public DateTime? PlannedAt { get; set; } public long? ResponsiblePartnerId { get; set; } public string? Notes { get; set; }
}
public sealed class TradeDocumentLink : Entity
{
    public long TradeDossierId { get; set; } public TradeDossier TradeDossier { get; set; } = null!; public TradeLinkType LinkType { get; set; } public long SourceId { get; set; } public long? SourceLineId { get; set; }
    public decimal? LinkedQuantity { get; set; } public string? ReferenceNumber { get; set; }
}
public sealed class CustomsDeclarationStatusHistory : Entity
{
    public long CustomsDeclarationId { get; set; } public CustomsDeclaration CustomsDeclaration { get; set; } = null!; public CustomsDeclarationStatus FromStatus { get; set; } public CustomsDeclarationStatus ToStatus { get; set; }
    public DateTime OccurredAt { get; set; } public string? ReasonCode { get; set; } public string? Notes { get; set; }
}
public sealed class TradeAttachment : Entity
{
    public long TradeDossierId { get; set; } public long? CustomsDeclarationId { get; set; } public TradeAttachmentType AttachmentType { get; set; } public string? DocumentNumber { get; set; } public DateOnly? DocumentDate { get; set; }
    public string OriginalFileName { get; set; } = string.Empty; public string StoredFileName { get; set; } = string.Empty; public string ContentType { get; set; } = string.Empty; public long FileSize { get; set; } public string Sha256 { get; set; } = string.Empty; public string? Notes { get; set; }
}
