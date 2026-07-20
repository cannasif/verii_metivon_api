using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class HardenInventoryPostingIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_DocumentType_DocumentId",
                table: "RII_INVENTORY_TRANSACTIONS",
                columns: new[] { "DocumentType", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_PostingId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "PostingId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_InventoryLotId",
                table: "RII_INVENTORY_BALANCES",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_InventorySerialId",
                table: "RII_INVENTORY_BALANCES",
                column: "InventorySerialId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_InventoryStatusId",
                table: "RII_INVENTORY_BALANCES",
                column: "InventoryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_StorageLocationId",
                table: "RII_INVENTORY_BALANCES",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_WarehouseId",
                table: "RII_INVENTORY_BALANCES",
                column: "WarehouseId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RII_INVENTORY_BALANCES_RESERVED",
                table: "RII_INVENTORY_BALANCES",
                sql: "[ReservedQuantity] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_INVENTORY_LOTS_InventoryLotId",
                table: "RII_INVENTORY_BALANCES",
                column: "InventoryLotId",
                principalTable: "RII_INVENTORY_LOTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_INVENTORY_SERIALS_InventorySerialId",
                table: "RII_INVENTORY_BALANCES",
                column: "InventorySerialId",
                principalTable: "RII_INVENTORY_SERIALS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_INVENTORY_STATUSES_InventoryStatusId",
                table: "RII_INVENTORY_BALANCES",
                column: "InventoryStatusId",
                principalTable: "RII_INVENTORY_STATUSES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_PRODUCTS_ProductId",
                table: "RII_INVENTORY_BALANCES",
                column: "ProductId",
                principalTable: "RII_PRODUCTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_STORAGE_LOCATIONS_StorageLocationId",
                table: "RII_INVENTORY_BALANCES",
                column: "StorageLocationId",
                principalTable: "RII_STORAGE_LOCATIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_WAREHOUSES_WarehouseId",
                table: "RII_INVENTORY_BALANCES",
                column: "WarehouseId",
                principalTable: "RII_WAREHOUSES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_INVENTORY_LOTS_InventoryLotId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_INVENTORY_SERIALS_InventorySerialId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_INVENTORY_STATUSES_InventoryStatusId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_PRODUCTS_ProductId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_STORAGE_LOCATIONS_StorageLocationId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_BALANCES_RII_WAREHOUSES_WarehouseId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_DocumentType_DocumentId",
                table: "RII_INVENTORY_TRANSACTIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_PostingId",
                table: "RII_INVENTORY_TRANSACTIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_InventoryLotId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_InventorySerialId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_InventoryStatusId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_StorageLocationId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_WarehouseId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RII_INVENTORY_BALANCES_RESERVED",
                table: "RII_INVENTORY_BALANCES");
        }
    }
}
