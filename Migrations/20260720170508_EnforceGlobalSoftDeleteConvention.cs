using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class EnforceGlobalSoftDeleteConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_WAREHOUSES_Code",
                table: "RII_WAREHOUSES");

            migrationBuilder.DropIndex(
                name: "IX_RII_WAREHOUSE_ZONES_WarehouseId_Code",
                table: "RII_WAREHOUSE_ZONES");

            migrationBuilder.DropIndex(
                name: "IX_RII_WAREHOUSE_TYPES_Code",
                table: "RII_WAREHOUSE_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_RII_USERS_Email",
                table: "RII_USERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_USERS_Username",
                table: "RII_USERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_USER_DETAILS_UserId",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropIndex(
                name: "IX_RII_TRANSFER_ORDERS_TransferNumber",
                table: "RII_TRANSFER_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_TransferOrderId_LineNumber",
                table: "RII_TRANSFER_ORDER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_STORAGE_LOCATIONS_Barcode",
                table: "RII_STORAGE_LOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_STORAGE_LOCATIONS_WarehouseId_Code",
                table: "RII_STORAGE_LOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SHIPMENTS_ShipmentNumber",
                table: "RII_SHIPMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SHIPMENT_LINES_ShipmentId_LineNumber",
                table: "RII_SHIPMENT_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_SALES_ORDERS_OrderNumber",
                table: "RII_SALES_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SALES_ORDER_LINES_SalesOrderId_LineNumber",
                table: "RII_SALES_ORDER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_PURCHASE_ORDERS_OrderNumber",
                table: "RII_PURCHASE_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_PurchaseOrderId_LineNumber",
                table: "RII_PURCHASE_ORDER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_PRICE_LISTS_Code",
                table: "RII_PRICE_LISTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_PRICE_LIST_LINES_PriceListId_ProductId_UnitId_CustomerGroupId_MinimumQuantity_ValidFrom",
                table: "RII_PRICE_LIST_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_LOCATION_TYPES_Code",
                table: "RII_LOCATION_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_Code",
                table: "RII_LEDGER_ACCOUNTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_JOURNAL_ENTRY_LINES_JournalEntryId_LineNumber",
                table: "RII_JOURNAL_ENTRY_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_JOURNAL_ENTRIES_JournalNumber",
                table: "RII_JOURNAL_ENTRIES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_IdempotencyKey",
                table: "RII_INVENTORY_TRANSACTIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_STATUSES_Code",
                table: "RII_INVENTORY_STATUSES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_SERIALS_ProductId_SerialNumber",
                table: "RII_INVENTORY_SERIALS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_RESERVATIONS_ReservationNumber",
                table: "RII_INVENTORY_RESERVATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_LOTS_ProductId_LotNumber",
                table: "RII_INVENTORY_LOTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COUNTS_CountNumber",
                table: "RII_INVENTORY_COUNTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COUNT_LINES_InventoryCountId_LineNumber",
                table: "RII_INVENTORY_COUNT_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COST_CLOSES_CloseNumber",
                table: "RII_INVENTORY_COST_CLOSES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_ProductId_WarehouseId_StorageLocationId_InventoryStatusId_InventoryLotId_InventorySerialId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ImportDossierId_LineNumber",
                table: "RII_IMPORT_DOSSIER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIER_ALLOCATIONS_ImportDossierCostId_ImportDossierLineId_Revision",
                table: "RII_IMPORT_DOSSIER_ALLOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPTS_ReceiptNumber",
                table: "RII_GOODS_RECEIPTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPT_SERIALS_GoodsReceiptLineId_SerialNumber",
                table: "RII_GOODS_RECEIPT_SERIALS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_GoodsReceiptId_LineNumber",
                table: "RII_GOODS_RECEIPT_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_DocumentType_DocumentNumber",
                table: "RII_ELECTRONIC_DOCUMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_Uuid",
                table: "RII_ELECTRONIC_DOCUMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_EDOCUMENT_PROVIDER_CONFIGURATIONS_BranchId_ProviderType_Environment",
                table: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_DISCOUNT_RULES_Code",
                table: "RII_DISCOUNT_RULES");

            migrationBuilder.DropIndex(
                name: "IX_RII_DELIVERY_NOTES_DeliveryNoteNumber",
                table: "RII_DELIVERY_NOTES");

            migrationBuilder.DropIndex(
                name: "IX_RII_DELIVERY_NOTES_GibUuid",
                table: "RII_DELIVERY_NOTES");

            migrationBuilder.DropIndex(
                name: "IX_RII_DELIVERY_NOTES_ShipmentId",
                table: "RII_DELIVERY_NOTES");

            migrationBuilder.DropIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_LINES_CustomsDeclarationId_LineNumber",
                table: "RII_CUSTOMS_DECLARATION_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_COMPANY_LEGAL_PROFILES_BranchId",
                table: "RII_COMPANY_LEGAL_PROFILES");

            migrationBuilder.DropIndex(
                name: "IX_RII_BRANCHES_Code",
                table: "RII_BRANCHES");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSES_Code",
                table: "RII_WAREHOUSES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_ZONES_WarehouseId_Code",
                table: "RII_WAREHOUSE_ZONES",
                columns: new[] { "WarehouseId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_TYPES_Code",
                table: "RII_WAREHOUSE_TYPES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_USERS_Email",
                table: "RII_USERS",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_USERS_Username",
                table: "RII_USERS",
                column: "Username",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_USER_DETAILS_UserId",
                table: "RII_USER_DETAILS",
                column: "UserId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDERS_TransferNumber",
                table: "RII_TRANSFER_ORDERS",
                column: "TransferNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_TransferOrderId_LineNumber",
                table: "RII_TRANSFER_ORDER_LINES",
                columns: new[] { "TransferOrderId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_Barcode",
                table: "RII_STORAGE_LOCATIONS",
                column: "Barcode",
                unique: true,
                filter: "([Barcode] IS NOT NULL) AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_WarehouseId_Code",
                table: "RII_STORAGE_LOCATIONS",
                columns: new[] { "WarehouseId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENTS_ShipmentNumber",
                table: "RII_SHIPMENTS",
                column: "ShipmentNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENT_LINES_ShipmentId_LineNumber",
                table: "RII_SHIPMENT_LINES",
                columns: new[] { "ShipmentId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDERS_OrderNumber",
                table: "RII_SALES_ORDERS",
                column: "OrderNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDER_LINES_SalesOrderId_LineNumber",
                table: "RII_SALES_ORDER_LINES",
                columns: new[] { "SalesOrderId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_OrderNumber",
                table: "RII_PURCHASE_ORDERS",
                column: "OrderNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_PurchaseOrderId_LineNumber",
                table: "RII_PURCHASE_ORDER_LINES",
                columns: new[] { "PurchaseOrderId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LISTS_Code",
                table: "RII_PRICE_LISTS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LIST_LINES_PriceListId_ProductId_UnitId_CustomerGroupId_MinimumQuantity_ValidFrom",
                table: "RII_PRICE_LIST_LINES",
                columns: new[] { "PriceListId", "ProductId", "UnitId", "CustomerGroupId", "MinimumQuantity", "ValidFrom" },
                unique: true,
                filter: "([CustomerGroupId] IS NOT NULL AND [ValidFrom] IS NOT NULL) AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LOCATION_TYPES_Code",
                table: "RII_LOCATION_TYPES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_Code",
                table: "RII_LEDGER_ACCOUNTS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRY_LINES_JournalEntryId_LineNumber",
                table: "RII_JOURNAL_ENTRY_LINES",
                columns: new[] { "JournalEntryId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_JournalNumber",
                table: "RII_JOURNAL_ENTRIES",
                column: "JournalNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_IdempotencyKey",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_STATUSES_Code",
                table: "RII_INVENTORY_STATUSES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_SERIALS_ProductId_SerialNumber",
                table: "RII_INVENTORY_SERIALS",
                columns: new[] { "ProductId", "SerialNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_RESERVATIONS_ReservationNumber",
                table: "RII_INVENTORY_RESERVATIONS",
                column: "ReservationNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_LOTS_ProductId_LotNumber",
                table: "RII_INVENTORY_LOTS",
                columns: new[] { "ProductId", "LotNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNTS_CountNumber",
                table: "RII_INVENTORY_COUNTS",
                column: "CountNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNT_LINES_InventoryCountId_LineNumber",
                table: "RII_INVENTORY_COUNT_LINES",
                columns: new[] { "InventoryCountId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COST_CLOSES_CloseNumber",
                table: "RII_INVENTORY_COST_CLOSES",
                column: "CloseNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_ProductId_WarehouseId_StorageLocationId_InventoryStatusId_InventoryLotId_InventorySerialId",
                table: "RII_INVENTORY_BALANCES",
                columns: new[] { "ProductId", "WarehouseId", "StorageLocationId", "InventoryStatusId", "InventoryLotId", "InventorySerialId" },
                unique: true,
                filter: "([InventoryLotId] IS NOT NULL AND [InventorySerialId] IS NOT NULL) AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ImportDossierId_LineNumber",
                table: "RII_IMPORT_DOSSIER_LINES",
                columns: new[] { "ImportDossierId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_ALLOCATIONS_ImportDossierCostId_ImportDossierLineId_Revision",
                table: "RII_IMPORT_DOSSIER_ALLOCATIONS",
                columns: new[] { "ImportDossierCostId", "ImportDossierLineId", "Revision" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPTS_ReceiptNumber",
                table: "RII_GOODS_RECEIPTS",
                column: "ReceiptNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_SERIALS_GoodsReceiptLineId_SerialNumber",
                table: "RII_GOODS_RECEIPT_SERIALS",
                columns: new[] { "GoodsReceiptLineId", "SerialNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_GoodsReceiptId_LineNumber",
                table: "RII_GOODS_RECEIPT_LINES",
                columns: new[] { "GoodsReceiptId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_DocumentType_DocumentNumber",
                table: "RII_ELECTRONIC_DOCUMENTS",
                columns: new[] { "DocumentType", "DocumentNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_Uuid",
                table: "RII_ELECTRONIC_DOCUMENTS",
                column: "Uuid",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_EDOCUMENT_PROVIDER_CONFIGURATIONS_BranchId_ProviderType_Environment",
                table: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS",
                columns: new[] { "BranchId", "ProviderType", "Environment" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DISCOUNT_RULES_Code",
                table: "RII_DISCOUNT_RULES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_DeliveryNoteNumber",
                table: "RII_DELIVERY_NOTES",
                column: "DeliveryNoteNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_GibUuid",
                table: "RII_DELIVERY_NOTES",
                column: "GibUuid",
                unique: true,
                filter: "([GibUuid] IS NOT NULL) AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_ShipmentId",
                table: "RII_DELIVERY_NOTES",
                column: "ShipmentId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_LINES_CustomsDeclarationId_LineNumber",
                table: "RII_CUSTOMS_DECLARATION_LINES",
                columns: new[] { "CustomsDeclarationId", "LineNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_COMPANY_LEGAL_PROFILES_BranchId",
                table: "RII_COMPANY_LEGAL_PROFILES",
                column: "BranchId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_BRANCHES_Code",
                table: "RII_BRANCHES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_WAREHOUSES_Code",
                table: "RII_WAREHOUSES");

            migrationBuilder.DropIndex(
                name: "IX_RII_WAREHOUSE_ZONES_WarehouseId_Code",
                table: "RII_WAREHOUSE_ZONES");

            migrationBuilder.DropIndex(
                name: "IX_RII_WAREHOUSE_TYPES_Code",
                table: "RII_WAREHOUSE_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_RII_USERS_Email",
                table: "RII_USERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_USERS_Username",
                table: "RII_USERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_USER_DETAILS_UserId",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropIndex(
                name: "IX_RII_TRANSFER_ORDERS_TransferNumber",
                table: "RII_TRANSFER_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_TransferOrderId_LineNumber",
                table: "RII_TRANSFER_ORDER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_STORAGE_LOCATIONS_Barcode",
                table: "RII_STORAGE_LOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_STORAGE_LOCATIONS_WarehouseId_Code",
                table: "RII_STORAGE_LOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SHIPMENTS_ShipmentNumber",
                table: "RII_SHIPMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SHIPMENT_LINES_ShipmentId_LineNumber",
                table: "RII_SHIPMENT_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_SALES_ORDERS_OrderNumber",
                table: "RII_SALES_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SALES_ORDER_LINES_SalesOrderId_LineNumber",
                table: "RII_SALES_ORDER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_PURCHASE_ORDERS_OrderNumber",
                table: "RII_PURCHASE_ORDERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_PurchaseOrderId_LineNumber",
                table: "RII_PURCHASE_ORDER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_PRICE_LISTS_Code",
                table: "RII_PRICE_LISTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_PRICE_LIST_LINES_PriceListId_ProductId_UnitId_CustomerGroupId_MinimumQuantity_ValidFrom",
                table: "RII_PRICE_LIST_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_LOCATION_TYPES_Code",
                table: "RII_LOCATION_TYPES");

            migrationBuilder.DropIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_Code",
                table: "RII_LEDGER_ACCOUNTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_JOURNAL_ENTRY_LINES_JournalEntryId_LineNumber",
                table: "RII_JOURNAL_ENTRY_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_JOURNAL_ENTRIES_JournalNumber",
                table: "RII_JOURNAL_ENTRIES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_IdempotencyKey",
                table: "RII_INVENTORY_TRANSACTIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_STATUSES_Code",
                table: "RII_INVENTORY_STATUSES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_SERIALS_ProductId_SerialNumber",
                table: "RII_INVENTORY_SERIALS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_RESERVATIONS_ReservationNumber",
                table: "RII_INVENTORY_RESERVATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_LOTS_ProductId_LotNumber",
                table: "RII_INVENTORY_LOTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COUNTS_CountNumber",
                table: "RII_INVENTORY_COUNTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COUNT_LINES_InventoryCountId_LineNumber",
                table: "RII_INVENTORY_COUNT_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COST_CLOSES_CloseNumber",
                table: "RII_INVENTORY_COST_CLOSES");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_BALANCES_ProductId_WarehouseId_StorageLocationId_InventoryStatusId_InventoryLotId_InventorySerialId",
                table: "RII_INVENTORY_BALANCES");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ImportDossierId_LineNumber",
                table: "RII_IMPORT_DOSSIER_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIER_ALLOCATIONS_ImportDossierCostId_ImportDossierLineId_Revision",
                table: "RII_IMPORT_DOSSIER_ALLOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPTS_ReceiptNumber",
                table: "RII_GOODS_RECEIPTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPT_SERIALS_GoodsReceiptLineId_SerialNumber",
                table: "RII_GOODS_RECEIPT_SERIALS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_GoodsReceiptId_LineNumber",
                table: "RII_GOODS_RECEIPT_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_DocumentType_DocumentNumber",
                table: "RII_ELECTRONIC_DOCUMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_Uuid",
                table: "RII_ELECTRONIC_DOCUMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_EDOCUMENT_PROVIDER_CONFIGURATIONS_BranchId_ProviderType_Environment",
                table: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS");

            migrationBuilder.DropIndex(
                name: "IX_RII_DISCOUNT_RULES_Code",
                table: "RII_DISCOUNT_RULES");

            migrationBuilder.DropIndex(
                name: "IX_RII_DELIVERY_NOTES_DeliveryNoteNumber",
                table: "RII_DELIVERY_NOTES");

            migrationBuilder.DropIndex(
                name: "IX_RII_DELIVERY_NOTES_GibUuid",
                table: "RII_DELIVERY_NOTES");

            migrationBuilder.DropIndex(
                name: "IX_RII_DELIVERY_NOTES_ShipmentId",
                table: "RII_DELIVERY_NOTES");

            migrationBuilder.DropIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_LINES_CustomsDeclarationId_LineNumber",
                table: "RII_CUSTOMS_DECLARATION_LINES");

            migrationBuilder.DropIndex(
                name: "IX_RII_COMPANY_LEGAL_PROFILES_BranchId",
                table: "RII_COMPANY_LEGAL_PROFILES");

            migrationBuilder.DropIndex(
                name: "IX_RII_BRANCHES_Code",
                table: "RII_BRANCHES");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSES_Code",
                table: "RII_WAREHOUSES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_ZONES_WarehouseId_Code",
                table: "RII_WAREHOUSE_ZONES",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_TYPES_Code",
                table: "RII_WAREHOUSE_TYPES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_USERS_Email",
                table: "RII_USERS",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_USERS_Username",
                table: "RII_USERS",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_USER_DETAILS_UserId",
                table: "RII_USER_DETAILS",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDERS_TransferNumber",
                table: "RII_TRANSFER_ORDERS",
                column: "TransferNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_ORDER_LINES_TransferOrderId_LineNumber",
                table: "RII_TRANSFER_ORDER_LINES",
                columns: new[] { "TransferOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_Barcode",
                table: "RII_STORAGE_LOCATIONS",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_WarehouseId_Code",
                table: "RII_STORAGE_LOCATIONS",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENTS_ShipmentNumber",
                table: "RII_SHIPMENTS",
                column: "ShipmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENT_LINES_ShipmentId_LineNumber",
                table: "RII_SHIPMENT_LINES",
                columns: new[] { "ShipmentId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDERS_OrderNumber",
                table: "RII_SALES_ORDERS",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDER_LINES_SalesOrderId_LineNumber",
                table: "RII_SALES_ORDER_LINES",
                columns: new[] { "SalesOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDERS_OrderNumber",
                table: "RII_PURCHASE_ORDERS",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PURCHASE_ORDER_LINES_PurchaseOrderId_LineNumber",
                table: "RII_PURCHASE_ORDER_LINES",
                columns: new[] { "PurchaseOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LISTS_Code",
                table: "RII_PRICE_LISTS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LIST_LINES_PriceListId_ProductId_UnitId_CustomerGroupId_MinimumQuantity_ValidFrom",
                table: "RII_PRICE_LIST_LINES",
                columns: new[] { "PriceListId", "ProductId", "UnitId", "CustomerGroupId", "MinimumQuantity", "ValidFrom" },
                unique: true,
                filter: "[CustomerGroupId] IS NOT NULL AND [ValidFrom] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LOCATION_TYPES_Code",
                table: "RII_LOCATION_TYPES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_Code",
                table: "RII_LEDGER_ACCOUNTS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRY_LINES_JournalEntryId_LineNumber",
                table: "RII_JOURNAL_ENTRY_LINES",
                columns: new[] { "JournalEntryId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_JournalNumber",
                table: "RII_JOURNAL_ENTRIES",
                column: "JournalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_TRANSACTIONS_IdempotencyKey",
                table: "RII_INVENTORY_TRANSACTIONS",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_STATUSES_Code",
                table: "RII_INVENTORY_STATUSES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_SERIALS_ProductId_SerialNumber",
                table: "RII_INVENTORY_SERIALS",
                columns: new[] { "ProductId", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_RESERVATIONS_ReservationNumber",
                table: "RII_INVENTORY_RESERVATIONS",
                column: "ReservationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_LOTS_ProductId_LotNumber",
                table: "RII_INVENTORY_LOTS",
                columns: new[] { "ProductId", "LotNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNTS_CountNumber",
                table: "RII_INVENTORY_COUNTS",
                column: "CountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNT_LINES_InventoryCountId_LineNumber",
                table: "RII_INVENTORY_COUNT_LINES",
                columns: new[] { "InventoryCountId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COST_CLOSES_CloseNumber",
                table: "RII_INVENTORY_COST_CLOSES",
                column: "CloseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_BALANCES_ProductId_WarehouseId_StorageLocationId_InventoryStatusId_InventoryLotId_InventorySerialId",
                table: "RII_INVENTORY_BALANCES",
                columns: new[] { "ProductId", "WarehouseId", "StorageLocationId", "InventoryStatusId", "InventoryLotId", "InventorySerialId" },
                unique: true,
                filter: "[InventoryLotId] IS NOT NULL AND [InventorySerialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ImportDossierId_LineNumber",
                table: "RII_IMPORT_DOSSIER_LINES",
                columns: new[] { "ImportDossierId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_ALLOCATIONS_ImportDossierCostId_ImportDossierLineId_Revision",
                table: "RII_IMPORT_DOSSIER_ALLOCATIONS",
                columns: new[] { "ImportDossierCostId", "ImportDossierLineId", "Revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPTS_ReceiptNumber",
                table: "RII_GOODS_RECEIPTS",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_SERIALS_GoodsReceiptLineId_SerialNumber",
                table: "RII_GOODS_RECEIPT_SERIALS",
                columns: new[] { "GoodsReceiptLineId", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPT_LINES_GoodsReceiptId_LineNumber",
                table: "RII_GOODS_RECEIPT_LINES",
                columns: new[] { "GoodsReceiptId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_DocumentType_DocumentNumber",
                table: "RII_ELECTRONIC_DOCUMENTS",
                columns: new[] { "DocumentType", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_Uuid",
                table: "RII_ELECTRONIC_DOCUMENTS",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_EDOCUMENT_PROVIDER_CONFIGURATIONS_BranchId_ProviderType_Environment",
                table: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS",
                columns: new[] { "BranchId", "ProviderType", "Environment" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_DISCOUNT_RULES_Code",
                table: "RII_DISCOUNT_RULES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_DeliveryNoteNumber",
                table: "RII_DELIVERY_NOTES",
                column: "DeliveryNoteNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_GibUuid",
                table: "RII_DELIVERY_NOTES",
                column: "GibUuid",
                unique: true,
                filter: "[GibUuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_ShipmentId",
                table: "RII_DELIVERY_NOTES",
                column: "ShipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_LINES_CustomsDeclarationId_LineNumber",
                table: "RII_CUSTOMS_DECLARATION_LINES",
                columns: new[] { "CustomsDeclarationId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_COMPANY_LEGAL_PROFILES_BranchId",
                table: "RII_COMPANY_LEGAL_PROFILES",
                column: "BranchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_BRANCHES_Code",
                table: "RII_BRANCHES",
                column: "Code",
                unique: true);
        }
    }
}
