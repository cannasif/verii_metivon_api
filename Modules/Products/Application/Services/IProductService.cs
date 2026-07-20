using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Modules.Products.Domain.Enums;

namespace verii_metivon_api.Modules.Products.Application.Services;

public sealed class ProductListQuery : PagedQuery
{
    public long? CategoryId { get; init; } public long? GroupId { get; init; } public long? BrandId { get; init; }
    public ProductType? ProductType { get; init; } public ProductLifecycleStatus? LifecycleStatus { get; init; }
    public InventoryTrackingType? TrackingType { get; init; } public bool? IsActive { get; init; }
}
public sealed record ProductListItem(long Id, string Code, string Name, string Category, string Group, string? Brand, string BaseUnit, ProductType ProductType, InventoryTrackingType TrackingType, ProductLifecycleStatus LifecycleStatus, bool IsActive);
public sealed record ProductUnitInput(long UnitId, decimal Numerator, decimal Denominator, bool IsPurchaseUnit, bool IsSalesUnit, bool IsDefaultPurchaseUnit, bool IsDefaultSalesUnit);
public sealed record ProductBarcodeInput(long? UnitId, string Barcode, BarcodeType BarcodeType, bool IsPrimary);
public sealed record ProductBranchInput(long BranchId, bool IsPurchasable, bool IsSellable, bool IsInventoryTracked, decimal? MinimumStock, decimal? MaximumStock, decimal? SafetyStock, decimal? ReorderPoint, decimal? ReorderQuantity, int? LeadTimeDays);
public sealed record ProductTranslationInput(string LanguageCode, string Name, string? ShortDescription, string? Description);
public sealed record SaveProductRequest(string Code, string Name, string? SearchName, string? Description, ProductType ProductType,
    ProductLifecycleStatus LifecycleStatus, InventoryTrackingType TrackingType, InventoryValuationMethod ValuationMethod, ProcurementType ProcurementType,
    long ProductCategoryId, long ProductGroupId, long? BrandId, long BaseUnitId, long PurchaseTaxGroupId, long SalesTaxGroupId,
    string? CountryOfOriginCode, string? CustomsTariffCode, string? ManufacturerCode, decimal? NetWeight, decimal? GrossWeight, decimal? Volume,
    int? ShelfLifeDays, bool? IsPurchasable, bool? IsSellable, bool? IsInventoryTracked,
    IReadOnlyList<ProductUnitInput>? Units, IReadOnlyList<ProductBarcodeInput>? Barcodes, IReadOnlyList<ProductBranchInput>? BranchSettings, IReadOnlyList<ProductTranslationInput>? Translations,
    long? BranchId);
public sealed record ProductDefinitions(IReadOnlyList<DefinitionItem> Categories, IReadOnlyList<DefinitionItem> Groups, IReadOnlyList<DefinitionItem> Brands,
    IReadOnlyList<DefinitionItem> UnitCategories, IReadOnlyList<DefinitionItem> Units, IReadOnlyList<DefinitionItem> PackageTypes, IReadOnlyList<DefinitionItem> TaxGroups);
public sealed class ProductDefinitionListQuery : PagedQuery { public bool? IsActive { get; init; } }
public sealed record ManagedProductDefinition(long Id,string Code,string Name,string? Description,bool IsActive,bool IsDefault,int DisplayOrder,long? ParentId,string? Website,long? UnitCategoryId,string? Symbol,int? DecimalPlaces,decimal? ConversionFactor,bool? IsBaseUnit,int? RoundingMethod,decimal? Length,decimal? Width,decimal? Height,decimal? EmptyWeight,long? DimensionUnitId,long? WeightUnitId);
public sealed record SaveProductDefinitionRequest(string Code,string Name,string? Description,bool IsActive,bool IsDefault,int DisplayOrder,long? ParentId,string? Website,long? UnitCategoryId,string? Symbol,int? DecimalPlaces,decimal? ConversionFactor,bool? IsBaseUnit,int? RoundingMethod,decimal? Length,decimal? Width,decimal? Height,decimal? EmptyWeight,long? DimensionUnitId,long? WeightUnitId);

public interface IProductService
{
    Task<ApiResponse<PagedResult<ProductListItem>>> GetPagedAsync(ProductListQuery query, string? culture, CancellationToken ct);
    Task<ApiResponse<ProductDefinitions>> GetDefinitionsAsync(string? culture, CancellationToken ct);
    Task<ApiResponse<object>> CreateAsync(SaveProductRequest request, string? culture, CancellationToken ct);
    Task<ApiResponse<SaveProductRequest>> GetByIdAsync(long id, CancellationToken ct);
    Task<ApiResponse<object>> UpdateAsync(long id, SaveProductRequest request, string? culture, CancellationToken ct);
    Task<ApiResponse<object>> DeleteAsync(long id, CancellationToken ct);
    Task<ApiResponse<PagedResult<ManagedProductDefinition>>> GetManagedDefinitionsAsync(string kind, ProductDefinitionListQuery query, string? culture, CancellationToken ct);
    Task<ApiResponse<object>> CreateDefinitionAsync(string kind, SaveProductDefinitionRequest request, string? culture, CancellationToken ct);
    Task<ApiResponse<object>> UpdateDefinitionAsync(string kind, long id, SaveProductDefinitionRequest request, string? culture, CancellationToken ct);
    Task<ApiResponse<object>> DeleteDefinitionAsync(string kind, long id, string? culture, CancellationToken ct);
}
