using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.Warehouses.Domain.Entities;

public sealed class WarehouseType : DefinitionEntity { }
public sealed class LocationType : DefinitionEntity { public bool IsPickable { get; set; } = true; public bool IsReceivable { get; set; } = true; }
public sealed class Warehouse : Entity
{
    public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty;
    public long BranchId { get; set; } public Branch Branch { get; set; } = null!;
    public long WarehouseTypeId { get; set; } public WarehouseType WarehouseType { get; set; } = null!;
    public string? Address { get; set; } public string? City { get; set; } public string? CountryCode { get; set; }
    public bool AllowNegativeStock { get; set; } public bool IsWmsEnabled { get; set; } = true;
    public bool IsActive { get; set; } = true; public bool IsDefault { get; set; }
    public ICollection<WarehouseZone> Zones { get; set; } = new List<WarehouseZone>();
}
public sealed class WarehouseZone : Entity
{
    public long WarehouseId { get; set; } public Warehouse Warehouse { get; set; } = null!;
    public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty;
    public int ZonePurpose { get; set; } public int PickPriority { get; set; } public int PutawayPriority { get; set; }
    public bool IsActive { get; set; } = true;
}
public sealed class StorageLocation : Entity
{
    public long WarehouseId { get; set; } public Warehouse Warehouse { get; set; } = null!;
    public long? WarehouseZoneId { get; set; } public WarehouseZone? WarehouseZone { get; set; }
    public long LocationTypeId { get; set; } public LocationType LocationType { get; set; } = null!;
    public string Code { get; set; } = string.Empty; public string? Barcode { get; set; }
    public string? Aisle { get; set; } public string? Bay { get; set; } public string? Level { get; set; } public string? Position { get; set; }
    public decimal? MaximumWeight { get; set; } public decimal? MaximumVolume { get; set; } public decimal? MaximumQuantity { get; set; }
    public bool IsReceiving { get; set; } public bool IsShipping { get; set; } public bool IsQuarantine { get; set; }
    public bool IsBlocked { get; set; } public bool IsActive { get; set; } = true;
}

public sealed class WarehouseParameterSettings : Entity
{
    public long? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public long? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public bool AllowNegativeStockByDefault { get; set; }
    public bool RequireLocation { get; set; } = true;
    public long? DefaultReceivingLocationId { get; set; }
    public long? DefaultQuarantineLocationId { get; set; }
    public long? DefaultShippingLocationId { get; set; }
    public bool UseDirectedPutAway { get; set; }
    public bool UseDirectedPicking { get; set; }
    public bool CheckCapacity { get; set; }
    public string LocationCodeFormat { get; set; } = "{WAREHOUSE}-{AISLE}-{RACK}-{BIN}";
    public int DefaultLabelCopies { get; set; } = 1;
    public byte[] RowVersion { get; set; } = [];
}
