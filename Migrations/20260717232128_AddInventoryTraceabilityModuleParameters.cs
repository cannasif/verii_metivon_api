using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTraceabilityModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_TRACEABILITY_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    RequireLotForLotTrackedProducts = table.Column<bool>(type: "bit", nullable: false),
                    RequireSerialForSerialTrackedProducts = table.Column<bool>(type: "bit", nullable: false),
                    RequireExpiryDateForShelfLifeProducts = table.Column<bool>(type: "bit", nullable: false),
                    RequireManufactureDateWhenExpiryExists = table.Column<bool>(type: "bit", nullable: false),
                    RejectExpiredReceipts = table.Column<bool>(type: "bit", nullable: false),
                    MinimumRemainingShelfLifeDays = table.Column<int>(type: "int", nullable: false),
                    MinimumRemainingShelfLifePercent = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    PreventDuplicateSerialNumbers = table.Column<bool>(type: "bit", nullable: false),
                    BlockExpiredShipments = table.Column<bool>(type: "bit", nullable: false),
                    EnforceFefoOnShipments = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryWarningDays = table.Column<int>(type: "int", nullable: false),
                    AllowMixedLotsPerDocument = table.Column<bool>(type: "bit", nullable: false),
                    AutoCreateLabelsAfterReceipt = table.Column<bool>(type: "bit", nullable: false),
                    DefaultLabelCopies = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_TRACEABILITY_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_TRACEABILITY_LABEL_COPIES", "[DefaultLabelCopies] >= 1 AND [DefaultLabelCopies] <= 100");
                    table.CheckConstraint("CK_RII_TRACEABILITY_SHELF_LIFE", "[MinimumRemainingShelfLifeDays] >= 0 AND [MinimumRemainingShelfLifePercent] >= 0 AND [MinimumRemainingShelfLifePercent] <= 100 AND [ExpiryWarningDays] >= 0");
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRACEABILITY_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRACEABILITY_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRACEABILITY_PARAMETERS_BranchId_WarehouseId",
                table: "RII_INVENTORY_TRACEABILITY_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRACEABILITY_PARAMETERS_WarehouseId",
                table: "RII_INVENTORY_TRACEABILITY_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_INVENTORY_TRACEABILITY_PARAMETERS");
        }
    }
}
