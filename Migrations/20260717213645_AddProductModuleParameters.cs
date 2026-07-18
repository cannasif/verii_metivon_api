using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_PRODUCT_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    ForceUppercase = table.Column<bool>(type: "bit", nullable: false),
                    TrimWhitespace = table.Column<bool>(type: "bit", nullable: false),
                    DefaultProductCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultProductGroupId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultBrandId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultBaseUnitId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultPurchaseTaxGroupId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultSalesTaxGroupId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultProductType = table.Column<int>(type: "int", nullable: false),
                    DefaultLifecycleStatus = table.Column<int>(type: "int", nullable: false),
                    DefaultTrackingType = table.Column<int>(type: "int", nullable: false),
                    DefaultValuationMethod = table.Column<int>(type: "int", nullable: false),
                    DefaultProcurementType = table.Column<int>(type: "int", nullable: false),
                    DefaultShelfLifeDays = table.Column<int>(type: "int", nullable: true),
                    DefaultPurchasable = table.Column<bool>(type: "bit", nullable: false),
                    DefaultSellable = table.Column<bool>(type: "bit", nullable: false),
                    DefaultInventoryTracked = table.Column<bool>(type: "bit", nullable: false),
                    RequireCountryOfOrigin = table.Column<bool>(type: "bit", nullable: false),
                    RequireCustomsTariffCode = table.Column<bool>(type: "bit", nullable: false),
                    RequireNetWeight = table.Column<bool>(type: "bit", nullable: false),
                    RequireGrossWeight = table.Column<bool>(type: "bit", nullable: false),
                    RequireShelfLife = table.Column<bool>(type: "bit", nullable: false),
                    MinimumOrderQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    MaximumOrderQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    OrderMultiple = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DefaultLeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    DefaultBarcodeType = table.Column<int>(type: "int", nullable: false),
                    MinimumRemainingShelfLifeDays = table.Column<int>(type: "int", nullable: false),
                    UseFefo = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
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
                    table.PrimaryKey("PK_RII_PRODUCT_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_PRODUCT_PARAMETERS_LEAD_TIME", "[DefaultLeadTimeDays] >= 0 AND [MinimumRemainingShelfLifeDays] >= 0");
                    table.CheckConstraint("CK_RII_PRODUCT_PARAMETERS_ORDER_QUANTITIES", "[MinimumOrderQuantity] >= 0 AND [MaximumOrderQuantity] >= 0 AND [OrderMultiple] > 0");
                    table.CheckConstraint("CK_RII_PRODUCT_PARAMETERS_SHELF_LIFE", "[DefaultShelfLifeDays] IS NULL OR [DefaultShelfLifeDays] >= 0");
                    table.ForeignKey(
                        name: "FK_RII_PRODUCT_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRODUCT_PARAMETERS_BranchId",
                table: "RII_PRODUCT_PARAMETERS",
                column: "BranchId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_PRODUCT_PARAMETERS");
        }
    }
}
