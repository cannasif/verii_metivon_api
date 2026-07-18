using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingAndCostingModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_ACCOUNTING_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    DefaultExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    MonetaryDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    JournalBalanceTolerance = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    RequireOpenFiscalPeriod = table.Column<bool>(type: "bit", nullable: false),
                    AllowBackdatedJournal = table.Column<bool>(type: "bit", nullable: false),
                    AllowFutureDatedJournal = table.Column<bool>(type: "bit", nullable: false),
                    AutoPostManualJournals = table.Column<bool>(type: "bit", nullable: false),
                    RequirePostingAccount = table.Column<bool>(type: "bit", nullable: false),
                    RequireJournalDescription = table.Column<bool>(type: "bit", nullable: false),
                    PreventDuplicateSourceDocument = table.Column<bool>(type: "bit", nullable: false),
                    DefaultInventoryValuationMethod = table.Column<int>(type: "int", nullable: false),
                    RequireInventoryPostingProfile = table.Column<bool>(type: "bit", nullable: false),
                    CreateCostLayersOnReceipt = table.Column<bool>(type: "bit", nullable: false),
                    AllowNegativeInventoryValue = table.Column<bool>(type: "bit", nullable: false),
                    CostDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    CostCloseTolerance = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    RequireApprovalBeforeCostClose = table.Column<bool>(type: "bit", nullable: false),
                    AllowCostCloseReopen = table.Column<bool>(type: "bit", nullable: false),
                    AutoCloseInventoryWithPeriod = table.Column<bool>(type: "bit", nullable: false),
                    IncludeLandedCostAdjustments = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_ACCOUNTING_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_ACCOUNTING_PARAMETERS_DECIMALS", "[MonetaryDecimalPlaces] >= 2 AND [MonetaryDecimalPlaces] <= 8 AND [CostDecimalPlaces] >= 2 AND [CostDecimalPlaces] <= 8");
                    table.CheckConstraint("CK_RII_ACCOUNTING_PARAMETERS_VALUES", "[DefaultExchangeRate] > 0 AND [JournalBalanceTolerance] >= 0 AND [CostCloseTolerance] >= 0");
                    table.ForeignKey(
                        name: "FK_RII_ACCOUNTING_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_ACCOUNTING_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_ACCOUNTING_PARAMETERS_BranchId_WarehouseId",
                table: "RII_ACCOUNTING_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_ACCOUNTING_PARAMETERS_WarehouseId",
                table: "RII_ACCOUNTING_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_ACCOUNTING_PARAMETERS");
        }
    }
}
