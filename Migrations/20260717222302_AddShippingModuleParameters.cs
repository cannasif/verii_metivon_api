using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_SHIPPING_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    AllowPartialShipment = table.Column<bool>(type: "bit", nullable: false),
                    RequireFullReservation = table.Column<bool>(type: "bit", nullable: false),
                    RequirePackedBeforeShipment = table.Column<bool>(type: "bit", nullable: false),
                    AutoMarkPackedOnCreate = table.Column<bool>(type: "bit", nullable: false),
                    AllowPastShipmentDate = table.Column<bool>(type: "bit", nullable: false),
                    RequireCarrierName = table.Column<bool>(type: "bit", nullable: false),
                    RequireVehiclePlate = table.Column<bool>(type: "bit", nullable: false),
                    RequireDriverName = table.Column<bool>(type: "bit", nullable: false),
                    RequireTrackingNumber = table.Column<bool>(type: "bit", nullable: false),
                    MaximumLinesPerShipment = table.Column<int>(type: "int", nullable: false),
                    RequireLotForLotTracked = table.Column<bool>(type: "bit", nullable: false),
                    RequireSerialForSerialTracked = table.Column<bool>(type: "bit", nullable: false),
                    AutoCreateDeliveryNote = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryNoteScenario = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DefaultDeliveryNoteStatus = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_SHIPPING_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_SHIPPING_PARAMETERS_MAX_LINES", "[MaximumLinesPerShipment] >= 1 AND [MaximumLinesPerShipment] <= 10000");
                    table.ForeignKey(
                        name: "FK_RII_SHIPPING_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_SHIPPING_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPPING_PARAMETERS_BranchId_WarehouseId",
                table: "RII_SHIPPING_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPPING_PARAMETERS_WarehouseId",
                table: "RII_SHIPPING_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_SHIPPING_PARAMETERS");
        }
    }
}
