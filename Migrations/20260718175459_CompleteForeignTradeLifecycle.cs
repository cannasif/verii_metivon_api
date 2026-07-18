using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class CompleteForeignTradeLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "RII_TRADE_DOSSIERS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryOfDispatchCode",
                table: "RII_TRADE_DOSSIERS",
                type: "varchar(2)",
                unicode: false,
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseTransportDocumentNumber",
                table: "RII_TRADE_DOSSIERS",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MasterTransportDocumentNumber",
                table: "RII_TRADE_DOSSIERS",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortOfDischargeCode",
                table: "RII_TRADE_DOSSIERS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortOfLoadingCode",
                table: "RII_TRADE_DOSSIERS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransportMode",
                table: "RII_TRADE_DOSSIERS",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "VehicleOrVoyageNumber",
                table: "RII_TRADE_DOSSIERS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "RII_CUSTOMS_DECLARATIONS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InspectionChannel",
                table: "RII_CUSTOMS_DECLARATIONS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "RII_CUSTOMS_DECLARATIONS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTaxes",
                table: "RII_CUSTOMS_DECLARATIONS",
                type: "decimal(24,8)",
                precision: 24,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "RII_CUSTOMS_DECLARATION_STATUS_HISTORY",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomsDeclarationId = table.Column<long>(type: "bigint", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_RII_CUSTOMS_DECLARATION_STATUS_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_CUSTOMS_DECLARATION_STATUS_HISTORY_RII_CUSTOMS_DECLARATIONS_CustomsDeclarationId",
                        column: x => x.CustomsDeclarationId,
                        principalTable: "RII_CUSTOMS_DECLARATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIERS_TradeDossierId",
                table: "RII_IMPORT_DOSSIERS",
                column: "TradeDossierId",
                unique: true,
                filter: "[TradeDossierId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATIONS_RegistrationNumber",
                table: "RII_CUSTOMS_DECLARATIONS",
                column: "RegistrationNumber",
                filter: "[RegistrationNumber] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_STATUS_HISTORY_CustomsDeclarationId_OccurredAt",
                table: "RII_CUSTOMS_DECLARATION_STATUS_HISTORY",
                columns: new[] { "CustomsDeclarationId", "OccurredAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_RII_IMPORT_DOSSIERS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_IMPORT_DOSSIERS",
                column: "TradeDossierId",
                principalTable: "RII_TRADE_DOSSIERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_IMPORT_DOSSIERS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_IMPORT_DOSSIERS");

            migrationBuilder.DropTable(
                name: "RII_CUSTOMS_DECLARATION_STATUS_HISTORY");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIERS_TradeDossierId",
                table: "RII_IMPORT_DOSSIERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_CUSTOMS_DECLARATIONS_RegistrationNumber",
                table: "RII_CUSTOMS_DECLARATIONS");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "CountryOfDispatchCode",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "HouseTransportDocumentNumber",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "MasterTransportDocumentNumber",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "PortOfDischargeCode",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "PortOfLoadingCode",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "TransportMode",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "VehicleOrVoyageNumber",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "RII_CUSTOMS_DECLARATIONS");

            migrationBuilder.DropColumn(
                name: "InspectionChannel",
                table: "RII_CUSTOMS_DECLARATIONS");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "RII_CUSTOMS_DECLARATIONS");

            migrationBuilder.DropColumn(
                name: "TotalTaxes",
                table: "RII_CUSTOMS_DECLARATIONS");
        }
    }
}
