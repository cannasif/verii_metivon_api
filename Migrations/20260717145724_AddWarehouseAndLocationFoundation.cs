using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseAndLocationFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_LOCATION_TYPES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsPickable = table.Column<bool>(type: "bit", nullable: false),
                    IsReceivable = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_LOCATION_TYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_WAREHOUSE_TYPES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_RII_WAREHOUSE_TYPES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_WAREHOUSES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowNegativeStock = table.Column<bool>(type: "bit", nullable: false),
                    IsWmsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_WAREHOUSES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_WAREHOUSES_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_WAREHOUSES_RII_WAREHOUSE_TYPES_WarehouseTypeId",
                        column: x => x.WarehouseTypeId,
                        principalTable: "RII_WAREHOUSE_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_WAREHOUSE_ZONES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZonePurpose = table.Column<int>(type: "int", nullable: false),
                    PickPriority = table.Column<int>(type: "int", nullable: false),
                    PutawayPriority = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_WAREHOUSE_ZONES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_WAREHOUSE_ZONES_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_STORAGE_LOCATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseZoneId = table.Column<long>(type: "bigint", nullable: true),
                    LocationTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Aisle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaximumWeight = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    MaximumVolume = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    MaximumQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    IsReceiving = table.Column<bool>(type: "bit", nullable: false),
                    IsShipping = table.Column<bool>(type: "bit", nullable: false),
                    IsQuarantine = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_STORAGE_LOCATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_STORAGE_LOCATIONS_RII_LOCATION_TYPES_LocationTypeId",
                        column: x => x.LocationTypeId,
                        principalTable: "RII_LOCATION_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_STORAGE_LOCATIONS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_STORAGE_LOCATIONS_RII_WAREHOUSE_ZONES_WarehouseZoneId",
                        column: x => x.WarehouseZoneId,
                        principalTable: "RII_WAREHOUSE_ZONES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_LOCATION_TYPES_Code",
                table: "RII_LOCATION_TYPES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_Barcode",
                table: "RII_STORAGE_LOCATIONS",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_LocationTypeId",
                table: "RII_STORAGE_LOCATIONS",
                column: "LocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_WarehouseId_Code",
                table: "RII_STORAGE_LOCATIONS",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_STORAGE_LOCATIONS_WarehouseZoneId",
                table: "RII_STORAGE_LOCATIONS",
                column: "WarehouseZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_TYPES_Code",
                table: "RII_WAREHOUSE_TYPES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSE_ZONES_WarehouseId_Code",
                table: "RII_WAREHOUSE_ZONES",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSES_BranchId",
                table: "RII_WAREHOUSES",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSES_Code",
                table: "RII_WAREHOUSES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_WAREHOUSES_WarehouseTypeId",
                table: "RII_WAREHOUSES",
                column: "WarehouseTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_STORAGE_LOCATIONS");

            migrationBuilder.DropTable(
                name: "RII_LOCATION_TYPES");

            migrationBuilder.DropTable(
                name: "RII_WAREHOUSE_ZONES");

            migrationBuilder.DropTable(
                name: "RII_WAREHOUSES");

            migrationBuilder.DropTable(
                name: "RII_WAREHOUSE_TYPES");
        }
    }
}
