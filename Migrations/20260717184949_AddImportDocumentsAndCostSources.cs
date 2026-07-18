using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddImportDocumentsAndCostSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "PaymentDate",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RII_IMPORT_DOSSIER_DOCUMENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDossierId = table.Column<long>(type: "bigint", nullable: false),
                    ImportDossierCostId = table.Column<long>(type: "bigint", nullable: true),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
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
                    table.PrimaryKey("PK_RII_IMPORT_DOSSIER_DOCUMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_DOCUMENTS_RII_IMPORT_DOSSIERS_ImportDossierId",
                        column: x => x.ImportDossierId,
                        principalTable: "RII_IMPORT_DOSSIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_IMPORT_DOSSIER_DOCUMENTS_RII_IMPORT_DOSSIER_COSTS_ImportDossierCostId",
                        column: x => x.ImportDossierCostId,
                        principalTable: "RII_IMPORT_DOSSIER_COSTS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_DOCUMENTS_ImportDossierCostId",
                table: "RII_IMPORT_DOSSIER_DOCUMENTS",
                column: "ImportDossierCostId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_DOCUMENTS_ImportDossierId_DocumentType",
                table: "RII_IMPORT_DOSSIER_DOCUMENTS",
                columns: new[] { "ImportDossierId", "DocumentType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_IMPORT_DOSSIER_DOCUMENTS");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "RII_IMPORT_DOSSIER_COSTS");
        }
    }
}
