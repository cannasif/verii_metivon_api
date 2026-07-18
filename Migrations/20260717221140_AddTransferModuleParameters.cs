using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_TRANSFER_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    AllowCrossBranchTransfer = table.Column<bool>(type: "bit", nullable: false),
                    RequireConfirmationBeforeShipment = table.Column<bool>(type: "bit", nullable: false),
                    AutoConfirmOnCreate = table.Column<bool>(type: "bit", nullable: false),
                    RequireExpectedReceiptDate = table.Column<bool>(type: "bit", nullable: false),
                    DefaultTransitDays = table.Column<int>(type: "int", nullable: false),
                    AllowPastTransferDate = table.Column<bool>(type: "bit", nullable: false),
                    AllowReceiptBeforeExpectedDate = table.Column<bool>(type: "bit", nullable: false),
                    RequireNotes = table.Column<bool>(type: "bit", nullable: false),
                    MaximumLinesPerTransfer = table.Column<int>(type: "int", nullable: false),
                    RequireLotForLotTracked = table.Column<bool>(type: "bit", nullable: false),
                    RequireSerialForSerialTracked = table.Column<bool>(type: "bit", nullable: false),
                    DefaultInventoryStatusId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_RII_TRANSFER_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_TRANSFER_PARAMETERS_MAX_LINES", "[MaximumLinesPerTransfer] >= 1 AND [MaximumLinesPerTransfer] <= 10000");
                    table.CheckConstraint("CK_RII_TRANSFER_PARAMETERS_TRANSIT_DAYS", "[DefaultTransitDays] >= 0 AND [DefaultTransitDays] <= 3650");
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_PARAMETERS_BranchId_WarehouseId",
                table: "RII_TRANSFER_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_PARAMETERS_WarehouseId",
                table: "RII_TRANSFER_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_TRANSFER_PARAMETERS");
        }
    }
}
