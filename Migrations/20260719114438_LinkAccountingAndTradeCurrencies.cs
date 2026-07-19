using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class LinkAccountingAndTradeCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "RII_TRADE_DOSSIERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "RII_LEDGER_ACCOUNTS",
                type: "varchar(3)",
                unicode: false,
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "RII_LEDGER_ACCOUNTS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "RII_JOURNAL_ENTRIES",
                type: "varchar(3)",
                unicode: false,
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "RII_JOURNAL_ENTRIES",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultCurrencyId",
                table: "RII_ACCOUNTING_PARAMETERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("""
                DECLARE @FallbackCurrencyId bigint = (
                    SELECT TOP (1) [Id]
                    FROM [RII_CURRENCIES]
                    ORDER BY CASE WHEN [IsoCode] = 'TRY' OR [Code] = 'TRY' THEN 0 ELSE 1 END, [Id]
                );

                IF @FallbackCurrencyId IS NULL
                    THROW 51000, 'Currency master data is required before linking financial records.', 1;

                UPDATE record
                SET [CurrencyId] = COALESCE(currencyByCode.[Id], @FallbackCurrencyId)
                FROM [RII_LEDGER_ACCOUNTS] record
                OUTER APPLY (
                    SELECT TOP (1) currency.[Id]
                    FROM [RII_CURRENCIES] currency
                    WHERE currency.[IsoCode] = record.[CurrencyCode] OR currency.[Code] = record.[CurrencyCode]
                    ORDER BY currency.[Id]
                ) currencyByCode;

                UPDATE record
                SET [CurrencyId] = COALESCE(currencyByCode.[Id], @FallbackCurrencyId)
                FROM [RII_JOURNAL_ENTRIES] record
                OUTER APPLY (
                    SELECT TOP (1) currency.[Id]
                    FROM [RII_CURRENCIES] currency
                    WHERE currency.[IsoCode] = record.[CurrencyCode] OR currency.[Code] = record.[CurrencyCode]
                    ORDER BY currency.[Id]
                ) currencyByCode;

                UPDATE record
                SET [CurrencyId] = COALESCE(currencyByCode.[Id], @FallbackCurrencyId)
                FROM [RII_TRADE_DOSSIERS] record
                OUTER APPLY (
                    SELECT TOP (1) currency.[Id]
                    FROM [RII_CURRENCIES] currency
                    WHERE currency.[IsoCode] = record.[CurrencyCode] OR currency.[Code] = record.[CurrencyCode]
                    ORDER BY currency.[Id]
                ) currencyByCode;

                UPDATE record
                SET [DefaultCurrencyId] = COALESCE(currencyByCode.[Id], @FallbackCurrencyId)
                FROM [RII_ACCOUNTING_PARAMETERS] record
                OUTER APPLY (
                    SELECT TOP (1) currency.[Id]
                    FROM [RII_CURRENCIES] currency
                    WHERE currency.[IsoCode] = record.[DefaultCurrencyCode] OR currency.[Code] = record.[DefaultCurrencyCode]
                    ORDER BY currency.[Id]
                ) currencyByCode;
                """);

            migrationBuilder.AlterColumn<long>(name: "CurrencyId", table: "RII_TRADE_DOSSIERS", type: "bigint", nullable: false, oldClrType: typeof(long), oldType: "bigint", oldNullable: true);
            migrationBuilder.AlterColumn<long>(name: "CurrencyId", table: "RII_LEDGER_ACCOUNTS", type: "bigint", nullable: false, oldClrType: typeof(long), oldType: "bigint", oldNullable: true);
            migrationBuilder.AlterColumn<long>(name: "CurrencyId", table: "RII_JOURNAL_ENTRIES", type: "bigint", nullable: false, oldClrType: typeof(long), oldType: "bigint", oldNullable: true);
            migrationBuilder.AlterColumn<long>(name: "DefaultCurrencyId", table: "RII_ACCOUNTING_PARAMETERS", type: "bigint", nullable: false, oldClrType: typeof(long), oldType: "bigint", oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_DOSSIERS_CurrencyId",
                table: "RII_TRADE_DOSSIERS",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_CurrencyId",
                table: "RII_LEDGER_ACCOUNTS",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_CurrencyId",
                table: "RII_JOURNAL_ENTRIES",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_ACCOUNTING_PARAMETERS_DefaultCurrencyId",
                table: "RII_ACCOUNTING_PARAMETERS",
                column: "DefaultCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RII_ACCOUNTING_PARAMETERS_RII_CURRENCIES_DefaultCurrencyId",
                table: "RII_ACCOUNTING_PARAMETERS",
                column: "DefaultCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_JOURNAL_ENTRIES_RII_CURRENCIES_CurrencyId",
                table: "RII_JOURNAL_ENTRIES",
                column: "CurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_LEDGER_ACCOUNTS_RII_CURRENCIES_CurrencyId",
                table: "RII_LEDGER_ACCOUNTS",
                column: "CurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_TRADE_DOSSIERS_RII_CURRENCIES_CurrencyId",
                table: "RII_TRADE_DOSSIERS",
                column: "CurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_ACCOUNTING_PARAMETERS_RII_CURRENCIES_DefaultCurrencyId",
                table: "RII_ACCOUNTING_PARAMETERS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_JOURNAL_ENTRIES_RII_CURRENCIES_CurrencyId",
                table: "RII_JOURNAL_ENTRIES");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_LEDGER_ACCOUNTS_RII_CURRENCIES_CurrencyId",
                table: "RII_LEDGER_ACCOUNTS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_TRADE_DOSSIERS_RII_CURRENCIES_CurrencyId",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_TRADE_DOSSIERS_CurrencyId",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_CurrencyId",
                table: "RII_LEDGER_ACCOUNTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_JOURNAL_ENTRIES_CurrencyId",
                table: "RII_JOURNAL_ENTRIES");

            migrationBuilder.DropIndex(
                name: "IX_RII_ACCOUNTING_PARAMETERS_DefaultCurrencyId",
                table: "RII_ACCOUNTING_PARAMETERS");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "RII_LEDGER_ACCOUNTS");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "RII_JOURNAL_ENTRIES");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyId",
                table: "RII_ACCOUNTING_PARAMETERS");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "RII_LEDGER_ACCOUNTS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldUnicode: false,
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "RII_JOURNAL_ENTRIES",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldUnicode: false,
                oldMaxLength: 3);
        }
    }
}
