using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class RefineNumberSeriesUsageAuditIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_DocumentNumber",
                table: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.DropIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_NumberSeriesId",
                table: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_NumberSeriesId_DocumentNumber",
                table: "RII_NUMBER_SERIES_USAGES",
                columns: new[] { "NumberSeriesId", "DocumentNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_NumberSeriesId_DocumentNumber",
                table: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_DocumentNumber",
                table: "RII_NUMBER_SERIES_USAGES",
                column: "DocumentNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_NumberSeriesId",
                table: "RII_NUMBER_SERIES_USAGES",
                column: "NumberSeriesId");
        }
    }
}
