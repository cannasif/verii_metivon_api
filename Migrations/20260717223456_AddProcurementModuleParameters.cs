using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurementModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_PROCUREMENT_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    RequireApprovalBeforeConfirmation = table.Column<bool>(type: "bit", nullable: false),
                    AutoApproveOnCreate = table.Column<bool>(type: "bit", nullable: false),
                    RequireRequestedDeliveryDate = table.Column<bool>(type: "bit", nullable: false),
                    DefaultLeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    AllowPastOrderDate = table.Column<bool>(type: "bit", nullable: false),
                    RequireSupplierReference = table.Column<bool>(type: "bit", nullable: false),
                    RequireBuyerNote = table.Column<bool>(type: "bit", nullable: false),
                    DefaultOverDeliveryTolerancePercent = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    DefaultUnderDeliveryTolerancePercent = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    MaximumOrderAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    MaximumLinesPerOrder = table.Column<int>(type: "int", nullable: false),
                    AllowZeroPrice = table.Column<bool>(type: "bit", nullable: false),
                    AllowDiscount = table.Column<bool>(type: "bit", nullable: false),
                    MaximumDiscountPercent = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    RequireActiveSupplier = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_PROCUREMENT_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_PROCUREMENT_PARAMETERS_AMOUNTS", "[MinimumOrderAmount] >= 0 AND [MaximumOrderAmount] >= 0 AND ([MaximumOrderAmount] = 0 OR [MaximumOrderAmount] >= [MinimumOrderAmount])");
                    table.CheckConstraint("CK_RII_PROCUREMENT_PARAMETERS_DISCOUNT", "[MaximumDiscountPercent] >= 0 AND [MaximumDiscountPercent] <= 100");
                    table.CheckConstraint("CK_RII_PROCUREMENT_PARAMETERS_LEAD_TIME", "[DefaultLeadTimeDays] >= 0 AND [DefaultLeadTimeDays] <= 3650");
                    table.CheckConstraint("CK_RII_PROCUREMENT_PARAMETERS_MAX_LINES", "[MaximumLinesPerOrder] >= 1 AND [MaximumLinesPerOrder] <= 10000");
                    table.CheckConstraint("CK_RII_PROCUREMENT_PARAMETERS_TOLERANCE", "[DefaultOverDeliveryTolerancePercent] >= 0 AND [DefaultOverDeliveryTolerancePercent] <= 100 AND [DefaultUnderDeliveryTolerancePercent] >= 0 AND [DefaultUnderDeliveryTolerancePercent] <= 100");
                    table.ForeignKey(
                        name: "FK_RII_PROCUREMENT_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PROCUREMENT_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_PROCUREMENT_PARAMETERS_BranchId_WarehouseId",
                table: "RII_PROCUREMENT_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PROCUREMENT_PARAMETERS_WarehouseId",
                table: "RII_PROCUREMENT_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_PROCUREMENT_PARAMETERS");
        }
    }
}
