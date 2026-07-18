using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_PRICING_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    PriceRoundingDecimals = table.Column<int>(type: "int", nullable: false),
                    DiscountRoundingDecimals = table.Column<int>(type: "int", nullable: false),
                    RoundingMethod = table.Column<int>(type: "int", nullable: false),
                    PreferCustomerGroupPrice = table.Column<bool>(type: "bit", nullable: false),
                    PreferHigherMinimumQuantity = table.Column<bool>(type: "bit", nullable: false),
                    PreferHigherPriceListPriority = table.Column<bool>(type: "bit", nullable: false),
                    FallbackToDefaultPriceList = table.Column<bool>(type: "bit", nullable: false),
                    AllowPriceListOverlap = table.Column<bool>(type: "bit", nullable: false),
                    AllowZeroPrice = table.Column<bool>(type: "bit", nullable: false),
                    AllowMultipleDiscounts = table.Column<bool>(type: "bit", nullable: false),
                    AllowFixedAmountDiscount = table.Column<bool>(type: "bit", nullable: false),
                    StopAtExclusiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    MaximumDiscountRulesApplied = table.Column<int>(type: "int", nullable: false),
                    MaximumTotalDiscountPercent = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
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
                    table.PrimaryKey("PK_RII_PRICING_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_PRICING_PARAMETERS_DISCOUNT_ROUNDING", "[DiscountRoundingDecimals] >= 0 AND [DiscountRoundingDecimals] <= 8");
                    table.CheckConstraint("CK_RII_PRICING_PARAMETERS_MAX_DISCOUNT", "[MaximumTotalDiscountPercent] >= 0 AND [MaximumTotalDiscountPercent] <= 100");
                    table.CheckConstraint("CK_RII_PRICING_PARAMETERS_MAX_RULES", "[MaximumDiscountRulesApplied] >= 1 AND [MaximumDiscountRulesApplied] <= 100");
                    table.CheckConstraint("CK_RII_PRICING_PARAMETERS_PRICE_ROUNDING", "[PriceRoundingDecimals] >= 0 AND [PriceRoundingDecimals] <= 8");
                    table.CheckConstraint("CK_RII_PRICING_PARAMETERS_ROUNDING_METHOD", "[RoundingMethod] >= 0 AND [RoundingMethod] <= 3");
                    table.ForeignKey(
                        name: "FK_RII_PRICING_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICING_PARAMETERS_BranchId",
                table: "RII_PRICING_PARAMETERS",
                column: "BranchId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_PRICING_PARAMETERS");
        }
    }
}
