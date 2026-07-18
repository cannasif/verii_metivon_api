using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberReservationLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RecycledAt",
                table: "RII_NUMBER_SERIES_USAGES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpiresAt",
                table: "RII_NUMBER_SERIES_USAGES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ReservationTimeoutMinutes",
                table: "RII_DOCUMENT_NUMBER_SERIES",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_Status_ReservationExpiresAt",
                table: "RII_NUMBER_SERIES_USAGES",
                columns: new[] { "Status", "ReservationExpiresAt" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_RII_DOCUMENT_NUMBER_SERIES_RESERVATION_TIMEOUT",
                table: "RII_DOCUMENT_NUMBER_SERIES",
                sql: "[ReservationTimeoutMinutes] >= 1 AND [ReservationTimeoutMinutes] <= 1440");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_Status_ReservationExpiresAt",
                table: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RII_DOCUMENT_NUMBER_SERIES_RESERVATION_TIMEOUT",
                table: "RII_DOCUMENT_NUMBER_SERIES");

            migrationBuilder.DropColumn(
                name: "RecycledAt",
                table: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.DropColumn(
                name: "ReservationExpiresAt",
                table: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.DropColumn(
                name: "ReservationTimeoutMinutes",
                table: "RII_DOCUMENT_NUMBER_SERIES");
        }
    }
}
