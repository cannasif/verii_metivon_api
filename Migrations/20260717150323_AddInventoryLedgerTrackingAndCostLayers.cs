using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryLedgerTrackingAndCostLayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_BALANCES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    PhysicalQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    InventoryValue = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_BALANCES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_LOTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    LotNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ManufactureDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BestBeforeDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SupplierLotNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_LOTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_LOTS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_RESERVATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationNumber = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: false),
                    SourceLineId = table.Column<long>(type: "bigint", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: true),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_RII_INVENTORY_RESERVATIONS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_STATUSES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsReservable = table.Column<bool>(type: "bit", nullable: false),
                    IsNettable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_INVENTORY_STATUSES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_SERIALS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_SERIALS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_SERIALS_RII_INVENTORY_LOTS_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "RII_INVENTORY_LOTS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_SERIALS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_TRANSACTIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentLineId = table.Column<long>(type: "bigint", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    BaseQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReversalOfId = table.Column<long>(type: "bigint", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_INVENTORY_TRANSACTIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_INVENTORY_LOTS_InventoryLotId",
                        column: x => x.InventoryLotId,
                        principalTable: "RII_INVENTORY_LOTS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_INVENTORY_SERIALS_InventorySerialId",
                        column: x => x.InventorySerialId,
                        principalTable: "RII_INVENTORY_SERIALS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_INVENTORY_STATUSES_InventoryStatusId",
                        column: x => x.InventoryStatusId,
                        principalTable: "RII_INVENTORY_STATUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_INVENTORY_TRANSACTIONS_ReversalOfId",
                        column: x => x.ReversalOfId,
                        principalTable: "RII_INVENTORY_TRANSACTIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_STORAGE_LOCATIONS_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "RII_STORAGE_LOCATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_TRANSACTIONS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_COST_LAYERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    ReceiptTransactionId = table.Column<long>(type: "bigint", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_COST_LAYERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_COST_LAYERS_RII_INVENTORY_TRANSACTIONS_ReceiptTransactionId",
                        column: x => x.ReceiptTransactionId,
                        principalTable: "RII_INVENTORY_TRANSACTIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_ProductId_WarehouseId_StorageLocationId_InventoryStatusId_InventoryLotId_InventorySerialId",
                table: "RII_INVENTORY_BALANCES",
                columns: new[] { "ProductId", "WarehouseId", "StorageLocationId", "InventoryStatusId", "InventoryLotId", "InventorySerialId" },
                unique: true,
                filter: "[InventoryLotId] IS NOT NULL AND [InventorySerialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COST_LAYERS_ProductId_WarehouseId_Status_ReceiptDate",
                table: "RII_INVENTORY_COST_LAYERS",
                columns: new[] { "ProductId", "WarehouseId", "Status", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COST_LAYERS_ReceiptTransactionId",
                table: "RII_INVENTORY_COST_LAYERS",
                column: "ReceiptTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_LOTS_ProductId_LotNumber",
                table: "RII_INVENTORY_LOTS",
                columns: new[] { "ProductId", "LotNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_RESERVATIONS_ReservationNumber",
                table: "RII_INVENTORY_RESERVATIONS",
                column: "ReservationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_RESERVATIONS_SourceType_SourceId_SourceLineId",
                table: "RII_INVENTORY_RESERVATIONS",
                columns: new[] { "SourceType", "SourceId", "SourceLineId" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_SERIALS_InventoryLotId",
                table: "RII_INVENTORY_SERIALS",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_SERIALS_ProductId_SerialNumber",
                table: "RII_INVENTORY_SERIALS",
                columns: new[] { "ProductId", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_STATUSES_Code",
                table: "RII_INVENTORY_STATUSES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_IdempotencyKey",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_InventoryLotId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "InventoryLotId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_InventorySerialId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "InventorySerialId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_InventoryStatusId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "InventoryStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_ProductId_WarehouseId_StorageLocationId_PostingDate",
                table: "RII_INVENTORY_TRANSACTIONS",
                columns: new[] { "ProductId", "WarehouseId", "StorageLocationId", "PostingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_ReversalOfId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "ReversalOfId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_StorageLocationId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_UnitId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_WarehouseId",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_COST_LAYERS");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_RESERVATIONS");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_TRANSACTIONS");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_SERIALS");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_STATUSES");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_LOTS");
        }
    }
}
