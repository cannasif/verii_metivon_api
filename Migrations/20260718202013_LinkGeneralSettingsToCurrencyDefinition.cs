using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class LinkGeneralSettingsToCurrencyDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE settings
                SET settings.DefaultCurrencyId = currency.Id,
                    settings.DefaultCurrencyCode = currency.IsoCode
                FROM RII_GENERAL_SETTINGS settings
                CROSS APPLY (
                    SELECT TOP (1) c.Id, c.IsoCode
                    FROM RII_CURRENCIES c
                    WHERE c.IsDeleted = 0
                      AND c.IsActive = 1
                    ORDER BY
                      CASE WHEN c.IsoCode = settings.DefaultCurrencyCode THEN 0
                           WHEN c.IsDefault = 1 THEN 1
                           ELSE 2 END,
                      c.DisplayOrder,
                      c.Id
                ) currency;
                """);

            migrationBuilder.AlterColumn<long>(
                name: "DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_GENERAL_SETTINGS_DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS",
                column: "DefaultCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RII_GENERAL_SETTINGS_RII_CURRENCIES_DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS",
                column: "DefaultCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_GENERAL_SETTINGS_RII_CURRENCIES_DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GENERAL_SETTINGS_DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyId",
                table: "RII_GENERAL_SETTINGS");
        }
    }
}
