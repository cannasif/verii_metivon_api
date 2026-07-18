using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEDocumentModuleParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_EDOCUMENT_PARAMETERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    ProviderEnvironment = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DefaultInvoiceScenario = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DefaultArchiveInvoiceScenario = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DefaultDespatchScenario = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AutoQueueAfterCreate = table.Column<bool>(type: "bit", nullable: false),
                    RequireUblValidation = table.Column<bool>(type: "bit", nullable: false),
                    RequireCompanyLegalProfile = table.Column<bool>(type: "bit", nullable: false),
                    RequireTaxNumberValidation = table.Column<bool>(type: "bit", nullable: false),
                    PreventDuplicateSourceDocument = table.Column<bool>(type: "bit", nullable: false),
                    FallbackToEArchive = table.Column<bool>(type: "bit", nullable: false),
                    RequireRecipientMailboxForEInvoice = table.Column<bool>(type: "bit", nullable: false),
                    SendEArchiveByEmail = table.Column<bool>(type: "bit", nullable: false),
                    AllowManualScenario = table.Column<bool>(type: "bit", nullable: false),
                    RetryDelayMinutes = table.Column<int>(type: "int", nullable: false),
                    StatusPollingIntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    ArchiveRetentionYears = table.Column<int>(type: "int", nullable: false),
                    MaximumXmlSizeKb = table.Column<int>(type: "int", nullable: false),
                    DefaultCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
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
                    table.PrimaryKey("PK_RII_EDOCUMENT_PARAMETERS", x => x.Id);
                    table.CheckConstraint("CK_RII_EDOCUMENT_PARAMETERS_RETENTION", "[ArchiveRetentionYears] >= 1 AND [ArchiveRetentionYears] <= 20 AND [MaximumXmlSizeKb] >= 64 AND [MaximumXmlSizeKb] <= 10240");
                    table.CheckConstraint("CK_RII_EDOCUMENT_PARAMETERS_TIMINGS", "[RetryDelayMinutes] >= 1 AND [RetryDelayMinutes] <= 1440 AND [StatusPollingIntervalMinutes] >= 1 AND [StatusPollingIntervalMinutes] <= 1440");
                    table.ForeignKey(
                        name: "FK_RII_EDOCUMENT_PARAMETERS_RII_BRANCHES_BranchId",
                        column: x => x.BranchId,
                        principalTable: "RII_BRANCHES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_EDOCUMENT_PARAMETERS_BranchId",
                table: "RII_EDOCUMENT_PARAMETERS",
                column: "BranchId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_EDOCUMENT_PARAMETERS");
        }
    }
}
