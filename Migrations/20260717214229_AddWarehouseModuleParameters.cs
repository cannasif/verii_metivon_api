using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_WAREHOUSE_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    AllowNegativeStockByDefault = table.Column<bool>(type: "bit", nullable: false),
                    RequireLocation = table.Column<bool>(type: "bit", nullable: false),
                    DefaultReceivingLocationId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultQuarantineLocationId = table.Column<long>(type: "bigint", nullable: true),
                    DefaultShippingLocationId = table.Column<long>(type: "bigint", nullable: true),
                    UseDirectedPutAway = table.Column<bool>(type: "bit", nullable: false),
                    UseDirectedPicking = table.Column<bool>(type: "bit", nullable: false),
                    CheckCapacity = table.Column<bool>(type: "bit", nullable: false),
                    LocationCodeFormat = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    DefaultLabelCopies = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_WAREHOUSE_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_WAREHOUSE_PARAMETERS_LABEL_COPIES", "[DefaultLabelCopies] >= 1 AND [DefaultLabelCopies] <= 100");
                    table.ForeignKey(
                        name: "FK_RII_WAREHOUSE_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_WAREHOUSE_PARAMETERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_PARAMETERS_BranchId_WarehouseId",
                table: "RII_WAREHOUSE_PARAMETERS",
                columns: new[] { "BranchId", "WarehouseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_PARAMETERS_WarehouseId",
                table: "RII_WAREHOUSE_PARAMETERS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_WAREHOUSE_PARAMETERS");
        }
    }
}
