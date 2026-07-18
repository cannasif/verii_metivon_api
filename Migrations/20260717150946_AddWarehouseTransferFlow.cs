using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseTransferFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_TRANSFER_ORDERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FromWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    ToWarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    TransferDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedReceiptDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ShippedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssuePostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceiptPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_TRANSFER_ORDERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_ORDERS_RII_WAREHOUSES_FromWarehouseId",
                        column: x => x.FromWarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_ORDERS_RII_WAREHOUSES_ToWarehouseId",
                        column: x => x.ToWarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_TRANSFER_ORDER_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferOrderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    FromLocationId = table.Column<long>(type: "bigint", nullable: false),
                    ToLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    RequestedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_TRANSFER_ORDER_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_ORDER_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_ORDER_LINES_RII_STORAGE_LOCATIONS_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "RII_STORAGE_LOCATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_ORDER_LINES_RII_STORAGE_LOCATIONS_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "RII_STORAGE_LOCATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_TRANSFER_ORDER_LINES_RII_TRANSFER_ORDERS_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "RII_TRANSFER_ORDERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_FromLocationId",
                table: "RII_TRANSFER_ORDER_LINES",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_ProductId",
                table: "RII_TRANSFER_ORDER_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_ToLocationId",
                table: "RII_TRANSFER_ORDER_LINES",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_TransferOrderId_LineNumber",
                table: "RII_TRANSFER_ORDER_LINES",
                columns: new[] { "TransferOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDERS_FromWarehouseId",
                table: "RII_TRANSFER_ORDERS",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDERS_ToWarehouseId",
                table: "RII_TRANSFER_ORDERS",
                column: "ToWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDERS_TransferNumber",
                table: "RII_TRANSFER_ORDERS",
                column: "TransferNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_TRANSFER_ORDER_LINES");

            migrationBuilder.DropTable(
                name: "RII_TRANSFER_ORDERS");
        }
    }
}
