using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_PURCHASE_ORDERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentTermId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ConfirmedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SupplierReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedBy = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_RII_PURCHASE_ORDERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDERS_RII_BUSINESS_PARTNERS_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "RII_BUSINESS_PARTNERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDERS_RII_CURRENCIES_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "RII_CURRENCIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDERS_RII_PAYMENT_TERMS_PaymentTermId",
                        column: x => x.PaymentTermId,
                        principalTable: "RII_PAYMENT_TERMS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PURCHASE_ORDER_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    InvoicedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    RequestedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OverDeliveryTolerance = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    UnderDeliveryTolerance = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_PURCHASE_ORDER_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDER_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDER_LINES_RII_PURCHASE_ORDERS_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "RII_PURCHASE_ORDERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDER_LINES_RII_STORAGE_LOCATIONS_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "RII_STORAGE_LOCATIONS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RII_PURCHASE_ORDER_LINES_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_ProductId",
                table: "RII_PURCHASE_ORDER_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_PurchaseOrderId_LineNumber",
                table: "RII_PURCHASE_ORDER_LINES",
                columns: new[] { "PurchaseOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_StorageLocationId",
                table: "RII_PURCHASE_ORDER_LINES",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_UnitId",
                table: "RII_PURCHASE_ORDER_LINES",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_BranchId",
                table: "RII_PURCHASE_ORDERS",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_CurrencyId",
                table: "RII_PURCHASE_ORDERS",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_OrderNumber",
                table: "RII_PURCHASE_ORDERS",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_PaymentTermId",
                table: "RII_PURCHASE_ORDERS",
                column: "PaymentTermId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_SupplierId",
                table: "RII_PURCHASE_ORDERS",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_WarehouseId",
                table: "RII_PURCHASE_ORDERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_PURCHASE_ORDER_LINES");

            migrationBuilder.DropTable(
                name: "RII_PURCHASE_ORDERS");
        }
    }
}
