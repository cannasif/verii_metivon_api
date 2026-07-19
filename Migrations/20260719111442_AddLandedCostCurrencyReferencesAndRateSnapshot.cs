using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddLandedCostCurrencyReferencesAndRateSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "RII_IMPORT_DOSSIERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExchangeRateDate",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExchangeRateSource",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalExchangeRate",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE dossier
                SET CurrencyId = currency.Id
                FROM RII_IMPORT_DOSSIERS dossier
                INNER JOIN RII_CURRENCIES currency
                    ON currency.IsoCode = dossier.CurrencyCode OR currency.Code = dossier.CurrencyCode;

                UPDATE cost
                SET CurrencyId = currency.Id,
                    OriginalExchangeRate = cost.ExchangeRate,
                    ExchangeRateSource = 'LegacyMigration'
                FROM RII_IMPORT_DOSSIER_COSTS cost
                INNER JOIN RII_CURRENCIES currency
                    ON currency.IsoCode = cost.CurrencyCode OR currency.Code = cost.CurrencyCode;

                DECLARE @FallbackCurrencyId bigint =
                    (SELECT TOP (1) Id FROM RII_CURRENCIES
                     ORDER BY CASE WHEN IsoCode = 'TRY' THEN 0 ELSE 1 END, Id);

                IF @FallbackCurrencyId IS NULL
                    THROW 51000, 'A currency definition is required before landed-cost currency migration.', 1;

                UPDATE RII_IMPORT_DOSSIERS
                SET CurrencyId = @FallbackCurrencyId
                WHERE CurrencyId IS NULL;

                UPDATE RII_IMPORT_DOSSIER_COSTS
                SET CurrencyId = @FallbackCurrencyId,
                    OriginalExchangeRate = CASE WHEN ExchangeRate > 0 THEN ExchangeRate ELSE 1 END,
                    ExchangeRateSource = COALESCE(ExchangeRateSource, 'LegacyMigration')
                WHERE CurrencyId IS NULL OR OriginalExchangeRate IS NULL;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "CurrencyId",
                table: "RII_IMPORT_DOSSIERS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalExchangeRate",
                table: "RII_IMPORT_DOSSIER_COSTS",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldPrecision: 18,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIERS_CurrencyId",
                table: "RII_IMPORT_DOSSIERS",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_IMPORT_DOSSIER_COSTS_CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RII_IMPORT_DOSSIER_COSTS_RII_CURRENCIES_CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS",
                column: "CurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_IMPORT_DOSSIERS_RII_CURRENCIES_CurrencyId",
                table: "RII_IMPORT_DOSSIERS",
                column: "CurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_IMPORT_DOSSIER_COSTS_RII_CURRENCIES_CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_IMPORT_DOSSIERS_RII_CURRENCIES_CurrencyId",
                table: "RII_IMPORT_DOSSIERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIERS_CurrencyId",
                table: "RII_IMPORT_DOSSIERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_IMPORT_DOSSIER_COSTS_CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "RII_IMPORT_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropColumn(
                name: "ExchangeRateSource",
                table: "RII_IMPORT_DOSSIER_COSTS");

            migrationBuilder.DropColumn(
                name: "OriginalExchangeRate",
                table: "RII_IMPORT_DOSSIER_COSTS");
        }
    }
}
