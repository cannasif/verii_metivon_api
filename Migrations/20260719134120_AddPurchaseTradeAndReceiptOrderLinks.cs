using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseTradeAndReceiptOrderLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TradeDossierId",
                table: "RII_PURCHASE_ORDERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RII_GOODS_RECEIPT_PURCHASE_ORDERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsReceiptId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RII_GOODS_RECEIPT_PURCHASE_ORDERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_PURCHASE_ORDERS_RII_GOODS_RECEIPTS_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "RII_GOODS_RECEIPTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_GOODS_RECEIPT_PURCHASE_ORDERS_RII_PURCHASE_ORDERS_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "RII_PURCHASE_ORDERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_TradeDossierId",
                table: "RII_PURCHASE_ORDERS",
                column: "TradeDossierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_PURCHASE_ORDERS_GoodsReceiptId_PurchaseOrderId",
                table: "RII_GOODS_RECEIPT_PURCHASE_ORDERS",
                columns: new[] { "GoodsReceiptId", "PurchaseOrderId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_PURCHASE_ORDERS_PurchaseOrderId",
                table: "RII_GOODS_RECEIPT_PURCHASE_ORDERS",
                column: "PurchaseOrderId");

            migrationBuilder.Sql("""
                INSERT INTO RII_GOODS_RECEIPT_PURCHASE_ORDERS
                    (GoodsReceiptId, PurchaseOrderId, CreatedAt, IsDeleted)
                SELECT Id, PurchaseOrderId, SYSUTCDATETIME(), 0
                FROM RII_GOODS_RECEIPTS
                WHERE PurchaseOrderId IS NOT NULL;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_PURCHASE_ORDERS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_PURCHASE_ORDERS",
                column: "TradeDossierId",
                principalTable: "RII_TRADE_DOSSIERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_PURCHASE_ORDERS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_PURCHASE_ORDERS");

            migrationBuilder.DropTable(
                name: "RII_GOODS_RECEIPT_PURCHASE_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_PURCHASE_ORDERS_TradeDossierId",
                table: "RII_PURCHASE_ORDERS");

            migrationBuilder.DropColumn(
                name: "TradeDossierId",
                table: "RII_PURCHASE_ORDERS");
        }
    }
}
