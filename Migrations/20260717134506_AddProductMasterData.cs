using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_BRANDS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Website = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_BRANDS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_CATEGORIES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_CATEGORIES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_CATEGORIES_RII_PRODUCT_CATEGORIES_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RII_PRODUCT_CATEGORIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_GROUPS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_GROUPS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_UNIT_CATEGORIES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_UNIT_CATEGORIES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_UNITS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "decimal(24,10)", precision: 24, scale: 10, nullable: false),
                    IsBaseUnit = table.Column<bool>(type: "bit", nullable: false),
                    RoundingMethod = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_UNITS", x => x.Id);
                    table.CheckConstraint("CK_RII_UNITS_CONVERSION_FACTOR", "[ConversionFactor] > 0");
                    table.CheckConstraint("CK_RII_UNITS_DECIMAL_PLACES", "[DecimalPlaces] >= 0 AND [DecimalPlaces] <= 10");
                    table.ForeignKey(
                        name: "FK_RII_UNITS_RII_UNIT_CATEGORIES_UnitCategoryId",
                        column: x => x.UnitCategoryId,
                        principalTable: "RII_UNIT_CATEGORIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PACKAGE_TYPES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Length = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    EmptyWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    DimensionUnitId = table.Column<long>(type: "bigint", nullable: true),
                    WeightUnitId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PACKAGE_TYPES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PACKAGE_TYPES_RII_UNITS_DimensionUnitId",
                        column: x => x.DimensionUnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PACKAGE_TYPES_RII_UNITS_WeightUnitId",
                        column: x => x.WeightUnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SearchName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    LifecycleStatus = table.Column<int>(type: "int", nullable: false),
                    TrackingType = table.Column<int>(type: "int", nullable: false),
                    ValuationMethod = table.Column<int>(type: "int", nullable: false),
                    ProcurementType = table.Column<int>(type: "int", nullable: false),
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ProductGroupId = table.Column<long>(type: "bigint", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: true),
                    BaseUnitId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseTaxGroupId = table.Column<long>(type: "bigint", nullable: false),
                    SalesTaxGroupId = table.Column<long>(type: "bigint", nullable: false),
                    CountryOfOriginCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    CustomsTariffCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ManufacturerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NetWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Volume = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    ShelfLifeDays = table.Column<int>(type: "int", nullable: true),
                    IsPurchasable = table.Column<bool>(type: "bit", nullable: false),
                    IsSellable = table.Column<bool>(type: "bit", nullable: false),
                    IsInventoryTracked = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCTS", x => x.Id);
                    table.CheckConstraint("CK_RII_PRODUCTS_SERVICE_INVENTORY", "[ProductType] <> 2 OR [IsInventoryTracked] = 0");
                    table.CheckConstraint("CK_RII_PRODUCTS_SHELF_LIFE", "[ShelfLifeDays] IS NULL OR [ShelfLifeDays] >= 0");
                    table.ForeignKey(
                        name: "FK_RII_PRODUCTS_RII_BRANDS_BrandId",
                        column: x => x.BrandId,
                        principalTable: "RII_BRANDS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCTS_RII_PRODUCT_CATEGORIES_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "RII_PRODUCT_CATEGORIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCTS_RII_PRODUCT_GROUPS_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "RII_PRODUCT_GROUPS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCTS_RII_TAX_GROUPS_PurchaseTaxGroupId",
                        column: x => x.PurchaseTaxGroupId,
                        principalTable: "RII_TAX_GROUPS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCTS_RII_TAX_GROUPS_SalesTaxGroupId",
                        column: x => x.SalesTaxGroupId,
                        principalTable: "RII_TAX_GROUPS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCTS_RII_UNITS_BaseUnitId",
                        column: x => x.BaseUnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_BARCODES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BarcodeType = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_BARCODES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_BARCODES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_BARCODES_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_BRANCH_SETTINGS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    IsPurchasable = table.Column<bool>(type: "bit", nullable: false),
                    IsSellable = table.Column<bool>(type: "bit", nullable: false),
                    IsInventoryTracked = table.Column<bool>(type: "bit", nullable: false),
                    MinimumStock = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    MaximumStock = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    SafetyStock = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    ReorderPoint = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    ReorderQuantity = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_BRANCH_SETTINGS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_BRANCH_SETTINGS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_BRANCH_SETTINGS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_PACKAGINGS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    PackageTypeId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(24,10)", precision: 24, scale: 10, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Length = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    IsReturnable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_PACKAGINGS", x => x.Id);
                    table.CheckConstraint("CK_RII_PRODUCT_PACKAGINGS_QUANTITY", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_PACKAGINGS_RII_PACKAGE_TYPES_PackageTypeId",
                        column: x => x.PackageTypeId,
                        principalTable: "RII_PACKAGE_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_PACKAGINGS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_PACKAGINGS_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_TRANSLATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_TRANSLATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_TRANSLATIONS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_UNITS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    Numerator = table.Column<decimal>(type: "decimal(24,10)", precision: 24, scale: 10, nullable: false),
                    Denominator = table.Column<decimal>(type: "decimal(24,10)", precision: 24, scale: 10, nullable: false),
                    IsPurchaseUnit = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesUnit = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultPurchaseUnit = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultSalesUnit = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRODUCT_UNITS", x => x.Id);
                    table.CheckConstraint("CK_RII_PRODUCT_UNITS_RATIO", "[Numerator] > 0 AND [Denominator] > 0");
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_UNITS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_UNITS_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_BRANDS_Code",
                table: "RII_BRANDS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PACKAGE_TYPES_Code",
                table: "RII_PACKAGE_TYPES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PACKAGE_TYPES_DimensionUnitId",
                table: "RII_PACKAGE_TYPES",
                column: "DimensionUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PACKAGE_TYPES_WeightUnitId",
                table: "RII_PACKAGE_TYPES",
                column: "WeightUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_BARCODES_Barcode",
                table: "RII_PRODUCT_BARCODES",
                column: "Barcode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_BARCODES_ProductId",
                table: "RII_PRODUCT_BARCODES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_BARCODES_UnitId",
                table: "RII_PRODUCT_BARCODES",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_BRANCH_SETTINGS_BranchId",
                table: "RII_PRODUCT_BRANCH_SETTINGS",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_BRANCH_SETTINGS_ProductId_BranchId",
                table: "RII_PRODUCT_BRANCH_SETTINGS",
                columns: new[] { "ProductId", "BranchId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_CATEGORIES_Code",
                table: "RII_PRODUCT_CATEGORIES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_CATEGORIES_ParentId",
                table: "RII_PRODUCT_CATEGORIES",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_GROUPS_Code",
                table: "RII_PRODUCT_GROUPS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_PACKAGINGS_PackageTypeId",
                table: "RII_PRODUCT_PACKAGINGS",
                column: "PackageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_PACKAGINGS_ProductId",
                table: "RII_PRODUCT_PACKAGINGS",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_PACKAGINGS_UnitId",
                table: "RII_PRODUCT_PACKAGINGS",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_TRANSLATIONS_ProductId_LanguageCode",
                table: "RII_PRODUCT_TRANSLATIONS",
                columns: new[] { "ProductId", "LanguageCode" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_UNITS_ProductId_UnitId",
                table: "RII_PRODUCT_UNITS",
                columns: new[] { "ProductId", "UnitId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_UNITS_UnitId",
                table: "RII_PRODUCT_UNITS",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_BaseUnitId",
                table: "RII_PRODUCTS",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_BrandId",
                table: "RII_PRODUCTS",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_Code",
                table: "RII_PRODUCTS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_Name",
                table: "RII_PRODUCTS",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_ProductCategoryId",
                table: "RII_PRODUCTS",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_ProductGroupId",
                table: "RII_PRODUCTS",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_PurchaseTaxGroupId",
                table: "RII_PRODUCTS",
                column: "PurchaseTaxGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_SalesTaxGroupId",
                table: "RII_PRODUCTS",
                column: "SalesTaxGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCTS_SearchName",
                table: "RII_PRODUCTS",
                column: "SearchName");

            migrationBuilder.CreateIndex(
                name: "IX_RII_UNIT_CATEGORIES_Code",
                table: "RII_UNIT_CATEGORIES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_UNITS_Code",
                table: "RII_UNITS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_UNITS_UnitCategoryId_Name",
                table: "RII_UNITS",
                columns: new[] { "UnitCategoryId", "Name" },
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_PRODUCT_BARCODES");

            migrationBuilder.DropTable(
                name: "RII_PRODUCT_BRANCH_SETTINGS");

            migrationBuilder.DropTable(
                name: "RII_PRODUCT_PACKAGINGS");

            migrationBuilder.DropTable(
                name: "RII_PRODUCT_TRANSLATIONS");

            migrationBuilder.DropTable(
                name: "RII_PRODUCT_UNITS");

            migrationBuilder.DropTable(
                name: "RII_PACKAGE_TYPES");

            migrationBuilder.DropTable(
                name: "RII_PRODUCTS");

            migrationBuilder.DropTable(
                name: "RII_BRANDS");

            migrationBuilder.DropTable(
                name: "RII_PRODUCT_CATEGORIES");

            migrationBuilder.DropTable(
                name: "RII_PRODUCT_GROUPS");

            migrationBuilder.DropTable(
                name: "RII_UNITS");

            migrationBuilder.DropTable(
                name: "RII_UNIT_CATEGORIES");
        }
    }
}
