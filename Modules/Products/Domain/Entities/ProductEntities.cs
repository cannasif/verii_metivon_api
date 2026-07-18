using verii_metivon_api.Core.Domain;
using verii_metivon_api.Modules.Products.Domain.Enums;

namespace verii_metivon_api.Modules.Products.Domain.Entities;

public sealed class ProductCategory : DefinitionEntity { public long? ParentId { get; set; } public ProductCategory? Parent { get; set; } }
public sealed class ProductGroup : DefinitionEntity { }
public sealed class Brand : DefinitionEntity { public string? Website { get; set; } }
public sealed class UnitCategory : DefinitionEntity { }
public sealed class Unit : DefinitionEntity
{
    public long UnitCategoryId { get; set; }
    public UnitCategory UnitCategory { get; set; } = null!;
    public string Symbol { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; }
    public decimal ConversionFactor { get; set; } = 1m;
    public bool IsBaseUnit { get; set; }
    public RoundingMethod RoundingMethod { get; set; }
}
public sealed class PackageType : DefinitionEntity
{
    public decimal? Length { get; set; } public decimal? Width { get; set; } public decimal? Height { get; set; }
    public decimal? EmptyWeight { get; set; }
    public long? DimensionUnitId { get; set; } public Unit? DimensionUnit { get; set; }
    public long? WeightUnitId { get; set; } public Unit? WeightUnit { get; set; }
}
public sealed class Product : Entity
{
    public string Code { get; set; } = string.Empty; public string Name { get; set; } = string.Empty;
    public string? SearchName { get; set; } public string? Description { get; set; }
    public ProductType ProductType { get; set; } = ProductType.Goods;
    public ProductLifecycleStatus LifecycleStatus { get; set; } = ProductLifecycleStatus.Draft;
    public InventoryTrackingType TrackingType { get; set; }
    public InventoryValuationMethod ValuationMethod { get; set; } = InventoryValuationMethod.MovingAverage;
    public ProcurementType ProcurementType { get; set; } = ProcurementType.Purchase;
    public long ProductCategoryId { get; set; } public ProductCategory ProductCategory { get; set; } = null!;
    public long ProductGroupId { get; set; } public ProductGroup ProductGroup { get; set; } = null!;
    public long? BrandId { get; set; } public Brand? Brand { get; set; }
    public long BaseUnitId { get; set; } public Unit BaseUnit { get; set; } = null!;
    public long PurchaseTaxGroupId { get; set; } public TaxGroup PurchaseTaxGroup { get; set; } = null!;
    public long SalesTaxGroupId { get; set; } public TaxGroup SalesTaxGroup { get; set; } = null!;
    public string? CountryOfOriginCode { get; set; } public string? CustomsTariffCode { get; set; } public string? ManufacturerCode { get; set; }
    public decimal? NetWeight { get; set; } public decimal? GrossWeight { get; set; } public decimal? Volume { get; set; }
    public int? ShelfLifeDays { get; set; }
    public bool IsPurchasable { get; set; } = true; public bool IsSellable { get; set; } = true;
    public bool IsInventoryTracked { get; set; } = true; public bool IsActive { get; set; } = true;
    public ICollection<ProductUnit> Units { get; set; } = new List<ProductUnit>();
    public ICollection<ProductBarcode> Barcodes { get; set; } = new List<ProductBarcode>();
    public ICollection<ProductPackaging> Packagings { get; set; } = new List<ProductPackaging>();
    public ICollection<ProductBranchSetting> BranchSettings { get; set; } = new List<ProductBranchSetting>();
    public ICollection<ProductTranslation> Translations { get; set; } = new List<ProductTranslation>();
}
public sealed class ProductUnit : Entity
{
    public long ProductId { get; set; } public Product Product { get; set; } = null!;
    public long UnitId { get; set; } public Unit Unit { get; set; } = null!;
    public decimal Numerator { get; set; } = 1m; public decimal Denominator { get; set; } = 1m;
    public bool IsPurchaseUnit { get; set; } public bool IsSalesUnit { get; set; }
    public bool IsDefaultPurchaseUnit { get; set; } public bool IsDefaultSalesUnit { get; set; }
    public bool IsActive { get; set; } = true;
}
public sealed class ProductBarcode : Entity
{
    public long ProductId { get; set; } public Product Product { get; set; } = null!;
    public long? UnitId { get; set; } public Unit? Unit { get; set; }
    public string Barcode { get; set; } = string.Empty; public BarcodeType BarcodeType { get; set; }
    public bool IsPrimary { get; set; } public bool IsActive { get; set; } = true;
}
public sealed class ProductPackaging : Entity
{
    public long ProductId { get; set; } public Product Product { get; set; } = null!;
    public long PackageTypeId { get; set; } public PackageType PackageType { get; set; } = null!;
    public long UnitId { get; set; } public Unit Unit { get; set; } = null!;
    public decimal Quantity { get; set; } public string? Barcode { get; set; }
    public decimal? Length { get; set; } public decimal? Width { get; set; } public decimal? Height { get; set; } public decimal? GrossWeight { get; set; }
    public bool IsReturnable { get; set; } public bool IsActive { get; set; } = true;
}
public sealed class ProductBranchSetting : Entity
{
    public long ProductId { get; set; } public Product Product { get; set; } = null!;
    public long BranchId { get; set; } public Branch Branch { get; set; } = null!;
    public bool IsPurchasable { get; set; } = true; public bool IsSellable { get; set; } = true; public bool IsInventoryTracked { get; set; } = true;
    public decimal? MinimumStock { get; set; } public decimal? MaximumStock { get; set; } public decimal? SafetyStock { get; set; }
    public decimal? ReorderPoint { get; set; } public decimal? ReorderQuantity { get; set; } public int? LeadTimeDays { get; set; }
    public bool IsActive { get; set; } = true;
}
public sealed class ProductTranslation : Entity
{
    public long ProductId { get; set; } public Product Product { get; set; } = null!;
    public string LanguageCode { get; set; } = string.Empty; public string Name { get; set; } = string.Empty;
    public string? ShortDescription { get; set; } public string? Description { get; set; }
}

public sealed class ProductParameterSettings : Entity
{
    public long? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public bool ForceUppercase { get; set; } = true;
    public bool TrimWhitespace { get; set; } = true;
    public long? DefaultProductCategoryId { get; set; }
    public long? DefaultProductGroupId { get; set; }
    public long? DefaultBrandId { get; set; }
    public long? DefaultBaseUnitId { get; set; }
    public long? DefaultPurchaseTaxGroupId { get; set; }
    public long? DefaultSalesTaxGroupId { get; set; }
    public ProductType DefaultProductType { get; set; } = ProductType.Goods;
    public ProductLifecycleStatus DefaultLifecycleStatus { get; set; } = ProductLifecycleStatus.Active;
    public InventoryTrackingType DefaultTrackingType { get; set; }
    public InventoryValuationMethod DefaultValuationMethod { get; set; } = InventoryValuationMethod.Fifo;
    public ProcurementType DefaultProcurementType { get; set; } = ProcurementType.Purchase;
    public int? DefaultShelfLifeDays { get; set; }
    public bool DefaultPurchasable { get; set; } = true;
    public bool DefaultSellable { get; set; } = true;
    public bool DefaultInventoryTracked { get; set; } = true;
    public bool RequireCountryOfOrigin { get; set; }
    public bool RequireCustomsTariffCode { get; set; }
    public bool RequireNetWeight { get; set; }
    public bool RequireGrossWeight { get; set; }
    public bool RequireShelfLife { get; set; }
    public decimal MinimumOrderQuantity { get; set; }
    public decimal MaximumOrderQuantity { get; set; }
    public decimal OrderMultiple { get; set; } = 1m;
    public int DefaultLeadTimeDays { get; set; }
    public BarcodeType DefaultBarcodeType { get; set; } = BarcodeType.Ean13;
    public int MinimumRemainingShelfLifeDays { get; set; }
    public bool UseFefo { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}
