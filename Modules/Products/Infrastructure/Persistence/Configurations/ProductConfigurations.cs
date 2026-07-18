using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Modules.Products.Domain.Entities;

namespace verii_metivon_api.Modules.Products.Infrastructure.Persistence.Configurations;

internal static class ProductDefinitionConfiguration
{
    public static void Configure<T>(EntityTypeBuilder<T> entity, string tableName) where T : DefinitionEntity
    {
        entity.ToTable(tableName); entity.HasKey(x => x.Id);
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        entity.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> e) { ProductDefinitionConfiguration.Configure(e, "RII_PRODUCT_CATEGORIES"); e.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict); }
}
public sealed class ProductGroupConfiguration : IEntityTypeConfiguration<ProductGroup>
{
    public void Configure(EntityTypeBuilder<ProductGroup> e) => ProductDefinitionConfiguration.Configure(e, "RII_PRODUCT_GROUPS");
}
public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> e) { ProductDefinitionConfiguration.Configure(e, "RII_BRANDS"); e.Property(x => x.Website).HasMaxLength(250); }
}
public sealed class UnitCategoryConfiguration : IEntityTypeConfiguration<UnitCategory>
{
    public void Configure(EntityTypeBuilder<UnitCategory> e) => ProductDefinitionConfiguration.Configure(e, "RII_UNIT_CATEGORIES");
}
public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> e)
    {
        ProductDefinitionConfiguration.Configure(e, "RII_UNITS");
        e.Property(x => x.Symbol).HasMaxLength(20).IsRequired(); e.Property(x => x.ConversionFactor).HasPrecision(24, 10);
        e.HasOne(x => x.UnitCategory).WithMany().HasForeignKey(x => x.UnitCategoryId).OnDelete(DeleteBehavior.Restrict);
        e.HasIndex(x => new { x.UnitCategoryId, x.Name }).HasFilter("[IsDeleted] = 0");
        e.ToTable(t =>
        {
            t.HasCheckConstraint("CK_RII_UNITS_DECIMAL_PLACES", "[DecimalPlaces] >= 0 AND [DecimalPlaces] <= 10");
            t.HasCheckConstraint("CK_RII_UNITS_CONVERSION_FACTOR", "[ConversionFactor] > 0");
        });
    }
}
public sealed class PackageTypeConfiguration : IEntityTypeConfiguration<PackageType>
{
    public void Configure(EntityTypeBuilder<PackageType> e)
    {
        ProductDefinitionConfiguration.Configure(e, "RII_PACKAGE_TYPES");
        e.Property(x => x.Length).HasPrecision(18, 6); e.Property(x => x.Width).HasPrecision(18, 6); e.Property(x => x.Height).HasPrecision(18, 6); e.Property(x => x.EmptyWeight).HasPrecision(18, 6);
        e.HasOne(x => x.DimensionUnit).WithMany().HasForeignKey(x => x.DimensionUnitId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.WeightUnit).WithMany().HasForeignKey(x => x.WeightUnitId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> e)
    {
        e.ToTable("RII_PRODUCTS", t =>
        {
            t.HasCheckConstraint("CK_RII_PRODUCTS_SERVICE_INVENTORY", "[ProductType] <> 2 OR [IsInventoryTracked] = 0");
            t.HasCheckConstraint("CK_RII_PRODUCTS_SHELF_LIFE", "[ShelfLifeDays] IS NULL OR [ShelfLifeDays] >= 0");
        });
        e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted);
        e.Property(x => x.Code).HasMaxLength(80).IsRequired(); e.Property(x => x.Name).HasMaxLength(250).IsRequired();
        e.Property(x => x.SearchName).HasMaxLength(250); e.Property(x => x.Description).HasMaxLength(2000);
        e.Property(x => x.CountryOfOriginCode).HasMaxLength(2); e.Property(x => x.CustomsTariffCode).HasMaxLength(20); e.Property(x => x.ManufacturerCode).HasMaxLength(100);
        e.Property(x => x.NetWeight).HasPrecision(18, 6); e.Property(x => x.GrossWeight).HasPrecision(18, 6); e.Property(x => x.Volume).HasPrecision(18, 6);
        e.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasIndex(x => x.Name); e.HasIndex(x => x.SearchName);
        e.HasOne(x => x.ProductCategory).WithMany().HasForeignKey(x => x.ProductCategoryId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.ProductGroup).WithMany().HasForeignKey(x => x.ProductGroupId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.Brand).WithMany().HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.BaseUnit).WithMany().HasForeignKey(x => x.BaseUnitId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.PurchaseTaxGroup).WithMany().HasForeignKey(x => x.PurchaseTaxGroupId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne(x => x.SalesTaxGroup).WithMany().HasForeignKey(x => x.SalesTaxGroupId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ProductUnitConfiguration : IEntityTypeConfiguration<ProductUnit>
{
    public void Configure(EntityTypeBuilder<ProductUnit> e)
    {
        e.ToTable("RII_PRODUCT_UNITS", t => t.HasCheckConstraint("CK_RII_PRODUCT_UNITS_RATIO", "[Numerator] > 0 AND [Denominator] > 0")); e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted);
        e.Property(x => x.Numerator).HasPrecision(24, 10); e.Property(x => x.Denominator).HasPrecision(24, 10);
        e.HasIndex(x => new { x.ProductId, x.UnitId }).IsUnique().HasFilter("[IsDeleted] = 0");
        e.HasOne(x => x.Product).WithMany(x => x.Units).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ProductBarcodeConfiguration : IEntityTypeConfiguration<ProductBarcode>
{
    public void Configure(EntityTypeBuilder<ProductBarcode> e)
    {
        e.ToTable("RII_PRODUCT_BARCODES"); e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted); e.Property(x => x.Barcode).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Barcode).IsUnique().HasFilter("[IsDeleted] = 0");
        e.HasOne(x => x.Product).WithMany(x => x.Barcodes).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ProductPackagingConfiguration : IEntityTypeConfiguration<ProductPackaging>
{
    public void Configure(EntityTypeBuilder<ProductPackaging> e)
    {
        e.ToTable("RII_PRODUCT_PACKAGINGS", t => t.HasCheckConstraint("CK_RII_PRODUCT_PACKAGINGS_QUANTITY", "[Quantity] > 0")); e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted);
        e.Property(x => x.Quantity).HasPrecision(24, 10); e.Property(x => x.Barcode).HasMaxLength(100);
        e.Property(x => x.Length).HasPrecision(18, 6); e.Property(x => x.Width).HasPrecision(18, 6); e.Property(x => x.Height).HasPrecision(18, 6); e.Property(x => x.GrossWeight).HasPrecision(18, 6);
        e.HasOne(x => x.Product).WithMany(x => x.Packagings).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        e.HasOne(x => x.PackageType).WithMany().HasForeignKey(x => x.PackageTypeId).OnDelete(DeleteBehavior.Restrict); e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ProductBranchSettingConfiguration : IEntityTypeConfiguration<ProductBranchSetting>
{
    public void Configure(EntityTypeBuilder<ProductBranchSetting> e)
    {
        e.ToTable("RII_PRODUCT_BRANCH_SETTINGS"); e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted);
        e.HasIndex(x => new { x.ProductId, x.BranchId }).IsUnique().HasFilter("[IsDeleted] = 0");
        foreach (var p in new[] { nameof(ProductBranchSetting.MinimumStock), nameof(ProductBranchSetting.MaximumStock), nameof(ProductBranchSetting.SafetyStock), nameof(ProductBranchSetting.ReorderPoint), nameof(ProductBranchSetting.ReorderQuantity) }) e.Property(p).HasPrecision(24, 6);
        e.HasOne(x => x.Product).WithMany(x => x.BranchSettings).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTranslation> e)
    {
        e.ToTable("RII_PRODUCT_TRANSLATIONS"); e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted);
        e.Property(x => x.LanguageCode).HasMaxLength(10).IsRequired(); e.Property(x => x.Name).HasMaxLength(250).IsRequired(); e.Property(x => x.ShortDescription).HasMaxLength(500); e.Property(x => x.Description).HasMaxLength(4000);
        e.HasIndex(x => new { x.ProductId, x.LanguageCode }).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasOne(x => x.Product).WithMany(x => x.Translations).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ProductParameterSettingsConfiguration : IEntityTypeConfiguration<ProductParameterSettings>
{
    public void Configure(EntityTypeBuilder<ProductParameterSettings> e)
    {
        e.ToTable("RII_PRODUCT_PARAMETERS", t =>
        {
            t.HasCheckConstraint("CK_RII_PRODUCT_PARAMETERS_SHELF_LIFE", "[DefaultShelfLifeDays] IS NULL OR [DefaultShelfLifeDays] >= 0");
            t.HasCheckConstraint("CK_RII_PRODUCT_PARAMETERS_ORDER_QUANTITIES", "[MinimumOrderQuantity] >= 0 AND [MaximumOrderQuantity] >= 0 AND [OrderMultiple] > 0");
            t.HasCheckConstraint("CK_RII_PRODUCT_PARAMETERS_LEAD_TIME", "[DefaultLeadTimeDays] >= 0 AND [MinimumRemainingShelfLifeDays] >= 0");
        });
        e.HasKey(x => x.Id); e.HasQueryFilter(x => !x.IsDeleted);
        e.HasIndex(x => x.BranchId).IsUnique().HasFilter("[IsDeleted] = 0");
        e.Property(x => x.MinimumOrderQuantity).HasPrecision(24, 8);
        e.Property(x => x.MaximumOrderQuantity).HasPrecision(24, 8);
        e.Property(x => x.OrderMultiple).HasPrecision(24, 8);
        e.Property(x => x.RowVersion).IsRowVersion();
        e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
