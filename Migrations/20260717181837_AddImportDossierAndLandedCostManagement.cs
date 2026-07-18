using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddImportDossierAndLandedCostManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_IMPORT_DOSSIERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IncotermCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CustomsDeclarationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomsDeclarationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TransactionExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    CustomsExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    CostingExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    OpenDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EstimatedArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ActualArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FinalizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllocationRevision = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_IMPORT_DOSSIERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIERS_RII_BUSINESS_PARTNERS_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "RII_BUSINESS_PARTNERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_LANDED_COST_TYPES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefaultAllocationMethod = table.Column<int>(type: "int", nullable: false),
                    IncludeInCustomsValue = table.Column<bool>(type: "bit", nullable: false),
                    CapitalizeToInventory = table.Column<bool>(type: "bit", nullable: false),
                    ClearingAccountId = table.Column<long>(type: "bigint", nullable: true),
                    VarianceAccountId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_LANDED_COST_TYPES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_LANDED_COST_TYPES_RII_LEDGER_ACCOUNTS_ClearingAccountId",
                        column: x => x.ClearingAccountId,
                        principalTable: "RII_LEDGER_ACCOUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_LANDED_COST_TYPES_RII_LEDGER_ACCOUNTS_VarianceAccountId",
                        column: x => x.VarianceAccountId,
                        principalTable: "RII_LEDGER_ACCOUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_IMPORT_DOSSIER_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDossierId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderLineId = table.Column<long>(type: "bigint", nullable: true),
                    GoodsReceiptLineId = table.Column<long>(type: "bigint", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ForeignUnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ForeignGoodsAmount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TransactionExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    CustomsExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    CostingExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    GoodsAmountLocal = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    AllocatedCostLocal = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    FinalUnitCostLocal = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ReceiptTransactionId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_RII_IMPORT_DOSSIER_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_LINES_RII_GOODS_RECEIPT_LINES_GoodsReceiptLineId",
                        column: x => x.GoodsReceiptLineId,
                        principalTable: "RII_GOODS_RECEIPT_LINES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_LINES_RII_IMPORT_DOSSIERS_ImportDossierId",
                        column: x => x.ImportDossierId,
                        principalTable: "RII_IMPORT_DOSSIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_LINES_RII_INVENTORY_TRANSACTIONS_ReceiptTransactionId",
                        column: x => x.ReceiptTransactionId,
                        principalTable: "RII_INVENTORY_TRANSACTIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_LINES_RII_PURCHASE_ORDER_LINES_PurchaseOrderLineId",
                        column: x => x.PurchaseOrderLineId,
                        principalTable: "RII_PURCHASE_ORDER_LINES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_IMPORT_DOSSIER_COSTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDossierId = table.Column<long>(type: "bigint", nullable: false),
                    LandedCostTypeId = table.Column<long>(type: "bigint", nullable: false),
                    AmountType = table.Column<int>(type: "int", nullable: false),
                    AllocationMethod = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ForeignAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    LocalAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_IMPORT_DOSSIER_COSTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_COSTS_RII_IMPORT_DOSSIERS_ImportDossierId",
                        column: x => x.ImportDossierId,
                        principalTable: "RII_IMPORT_DOSSIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_COSTS_RII_LANDED_COST_TYPES_LandedCostTypeId",
                        column: x => x.LandedCostTypeId,
                        principalTable: "RII_LANDED_COST_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_IMPORT_DOSSIER_ALLOCATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDossierCostId = table.Column<long>(type: "bigint", nullable: false),
                    ImportDossierLineId = table.Column<long>(type: "bigint", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    BasisValue = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    AllocationRate = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    IsManual = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_IMPORT_DOSSIER_ALLOCATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_ALLOCATIONS_RII_IMPORT_DOSSIER_COSTS_ImportDossierCostId",
                        column: x => x.ImportDossierCostId,
                        principalTable: "RII_IMPORT_DOSSIER_COSTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_ALLOCATIONS_RII_IMPORT_DOSSIER_LINES_ImportDossierLineId",
                        column: x => x.ImportDossierLineId,
                        principalTable: "RII_IMPORT_DOSSIER_LINES",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_ALLOCATIONS_ImportDossierCostId_ImportDossierLineId_Revision",
                table: "RII_IMPORT_DOSSIER_ALLOCATIONS",
                columns: new[] { "ImportDossierCostId", "ImportDossierLineId", "Revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_ALLOCATIONS_ImportDossierLineId",
                table: "RII_IMPORT_DOSSIER_ALLOCATIONS",
                column: "ImportDossierLineId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_COSTS_ImportDossierId",
                table: "RII_IMPORT_DOSSIER_COSTS",
                column: "ImportDossierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_COSTS_LandedCostTypeId",
                table: "RII_IMPORT_DOSSIER_COSTS",
                column: "LandedCostTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_GoodsReceiptLineId",
                table: "RII_IMPORT_DOSSIER_LINES",
                column: "GoodsReceiptLineId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ImportDossierId_LineNumber",
                table: "RII_IMPORT_DOSSIER_LINES",
                columns: new[] { "ImportDossierId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ProductId",
                table: "RII_IMPORT_DOSSIER_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_PurchaseOrderLineId",
                table: "RII_IMPORT_DOSSIER_LINES",
                column: "PurchaseOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_LINES_ReceiptTransactionId",
                table: "RII_IMPORT_DOSSIER_LINES",
                column: "ReceiptTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIERS_DossierNumber",
                table: "RII_IMPORT_DOSSIERS",
                column: "DossierNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIERS_SupplierId",
                table: "RII_IMPORT_DOSSIERS",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LANDED_COST_TYPES_ClearingAccountId",
                table: "RII_LANDED_COST_TYPES",
                column: "ClearingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LANDED_COST_TYPES_Code",
                table: "RII_LANDED_COST_TYPES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LANDED_COST_TYPES_VarianceAccountId",
                table: "RII_LANDED_COST_TYPES",
                column: "VarianceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_IMPORT_DOSSIER_ALLOCATIONS");

            migrationBuilder.DropTable(
                name: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropTable(
                name: "RII_IMPORT_DOSSIER_LINES");

            migrationBuilder.DropTable(
                name: "RII_LANDED_COST_TYPES");

            migrationBuilder.DropTable(
                name: "RII_IMPORT_DOSSIERS");
        }
    }
}
