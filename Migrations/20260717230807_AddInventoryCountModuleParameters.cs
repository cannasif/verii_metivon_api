using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryCountModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_COUNT_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultBlindCount = table.Column<bool>(type: "bit", nullable: false),
                    FreezeOnCreate = table.Column<bool>(type: "bit", nullable: false),
                    AllowPastCountDate = table.Column<bool>(type: "bit", nullable: false),
                    RequireStorageLocation = table.Column<bool>(type: "bit", nullable: false),
                    IncludeZeroBalances = table.Column<bool>(type: "bit", nullable: false),
                    PreventOverlappingOpenCounts = table.Column<bool>(type: "bit", nullable: false),
                    MaximumLinesPerCount = table.Column<int>(type: "int", nullable: false),
                    AllowNegativeCountedQuantity = table.Column<bool>(type: "bit", nullable: false),
                    RequireReasonCodeForDifference = table.Column<bool>(type: "bit", nullable: false),
                    AutomaticApprovalQuantityTolerance = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    AutomaticApprovalPercentTolerance = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    AutoApproveWithinTolerance = table.Column<bool>(type: "bit", nullable: false),
                    RequireApprovalBeforePosting = table.Column<bool>(type: "bit", nullable: false),
                    AllowPostingWithoutDifference = table.Column<bool>(type: "bit", nullable: false),
                    PostingCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_COUNT_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_INVENTORY_COUNT_PARAMETERS_MAX_LINES", "[MaximumLinesPerCount] >= 1 AND [MaximumLinesPerCount] <= 100000");
                    table.CheckConstraint("CK_RII_INVENTORY_COUNT_PARAMETERS_TOLERANCE", "[AutomaticApprovalQuantityTolerance] >= 0 AND [AutomaticApprovalPercentTolerance] >= 0 AND [AutomaticApprovalPercentTolerance] <= 100");
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_COUNT_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_COUNT_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNT_PARAMETERS_BranchId_WarehouseId",
                table: "RII_INVENTORY_COUNT_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNT_PARAMETERS_WarehouseId",
                table: "RII_INVENTORY_COUNT_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_INVENTORY_COUNT_PARAMETERS");
        }
    }
}
