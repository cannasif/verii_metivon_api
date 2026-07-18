using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceiptFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_GOODS_RECEIPTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ReceiptType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierDeliveryNoteNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InventoryPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_RII_GOODS_RECEIPTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPTS_RII_PURCHASE_ORDERS_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "RII_PURCHASE_ORDERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPTS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_GOODS_RECEIPT_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderLineId = table.Column<long>(type: "bigint", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    ExpectedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    AcceptedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    RejectedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    LotNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_RII_GOODS_RECEIPT_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_LINES_RII_GOODS_RECEIPTS_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "RII_GOODS_RECEIPTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_LINES_RII_PURCHASE_ORDER_LINES_PurchaseOrderLineId",
                        column: x => x.PurchaseOrderLineId,
                        principalTable: "RII_PURCHASE_ORDER_LINES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_LINES_RII_STORAGE_LOCATIONS_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "RII_STORAGE_LOCATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_LINES_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_GOODS_RECEIPT_SERIALS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptLineId = table.Column<long>(type: "bigint", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
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
                    table.PrimaryKey("PK_RII_GOODS_RECEIPT_SERIALS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_SERIALS_RII_GOODS_RECEIPT_LINES_GoodsReceiptLineId",
                        column: x => x.GoodsReceiptLineId,
                        principalTable: "RII_GOODS_RECEIPT_LINES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_GoodsReceiptId_LineNumber",
                table: "RII_GOODS_RECEIPT_LINES",
                columns: new[] { "GoodsReceiptId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_ProductId",
                table: "RII_GOODS_RECEIPT_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_PurchaseOrderLineId",
                table: "RII_GOODS_RECEIPT_LINES",
                column: "PurchaseOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_StorageLocationId",
                table: "RII_GOODS_RECEIPT_LINES",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_UnitId",
                table: "RII_GOODS_RECEIPT_LINES",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_SERIALS_GoodsReceiptLineId_SerialNumber",
                table: "RII_GOODS_RECEIPT_SERIALS",
                columns: new[] { "GoodsReceiptLineId", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPTS_PurchaseOrderId",
                table: "RII_GOODS_RECEIPTS",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPTS_ReceiptNumber",
                table: "RII_GOODS_RECEIPTS",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPTS_WarehouseId",
                table: "RII_GOODS_RECEIPTS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_GOODS_RECEIPT_SERIALS");

            migrationBuilder.DropTable(
                name: "RII_GOODS_RECEIPT_LINES");

            migrationBuilder.DropTable(
                name: "RII_GOODS_RECEIPTS");
        }
    }
}
