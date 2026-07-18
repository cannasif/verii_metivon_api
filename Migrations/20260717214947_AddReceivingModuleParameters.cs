using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddReceivingModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_RECEIVING_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    RequirePurchaseOrder = table.Column<bool>(type: "bit", nullable: false),
                    AllowFreeReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AllowPartialReceipt = table.Column<bool>(type: "bit", nullable: false),
                    OverDeliveryTolerancePercent = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false),
                    UnderDeliveryTolerancePercent = table.Column<decimal>(type: "decimal(7,4)", precision: 7, scale: 4, nullable: false),
                    RequireSupplierDeliveryNoteNumber = table.Column<bool>(type: "bit", nullable: false),
                    RequireLotNumberForLotTracked = table.Column<bool>(type: "bit", nullable: false),
                    RequireSerialsForSerialTracked = table.Column<bool>(type: "bit", nullable: false),
                    RequireExpiryDateForTrackedItems = table.Column<bool>(type: "bit", nullable: false),
                    MinimumRemainingShelfLifeDays = table.Column<int>(type: "int", nullable: false),
                    RequireQualityInspection = table.Column<bool>(type: "bit", nullable: false),
                    AutoCreateLabels = table.Column<bool>(type: "bit", nullable: false),
                    DefaultLabelCopies = table.Column<int>(type: "int", nullable: false),
                    InventoryCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
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
                    table.PrimaryKey("PK_RII_RECEIVING_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_RECEIVING_PARAMETERS_LABEL_COPIES", "[DefaultLabelCopies] >= 1 AND [DefaultLabelCopies] <= 100");
                    table.CheckConstraint("CK_RII_RECEIVING_PARAMETERS_SHELF_LIFE", "[MinimumRemainingShelfLifeDays] >= 0");
                    table.CheckConstraint("CK_RII_RECEIVING_PARAMETERS_TOLERANCE", "[OverDeliveryTolerancePercent] >= 0 AND [OverDeliveryTolerancePercent] <= 100 AND [UnderDeliveryTolerancePercent] >= 0 AND [UnderDeliveryTolerancePercent] <= 100");
                    table.ForeignKey(
                        name: "FK_RII_RECEIVING_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_RECEIVING_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_RECEIVING_PARAMETERS_BranchId_WarehouseId",
                table: "RII_RECEIVING_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_RECEIVING_PARAMETERS_WarehouseId",
                table: "RII_RECEIVING_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_RECEIVING_PARAMETERS");
        }
    }
}
