using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_GENERAL_SETTINGS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScopeKey = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumberFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AmountDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    PriceDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    QuantityDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    ExchangeRateDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    PercentageDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    CostDecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    RoundingMethod = table.Column<int>(type: "int", nullable: false),
                    UseThousandsSeparator = table.Column<bool>(type: "bit", nullable: false),
                    TrimTrailingZeros = table.Column<bool>(type: "bit", nullable: false),
                    DefaultCurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    CurrencyDisplay = table.Column<int>(type: "int", nullable: false),
                    CurrencySymbolOnRight = table.Column<bool>(type: "bit", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TimeFormat = table.Column<int>(type: "int", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstDayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StoreDateTimeAsUtc = table.Column<bool>(type: "bit", nullable: false),
                    DefaultPageSize = table.Column<int>(type: "int", nullable: false),
                    MaximumExportRows = table.Column<int>(type: "int", nullable: false),
                    AllowUserDisplayOverrides = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_GENERAL_SETTINGS", x => x.Id);
                    table.CheckConstraint("CK_RII_GENERAL_SETTINGS_DECIMALS", "[AmountDecimalPlaces] BETWEEN 0 AND 8 AND [PriceDecimalPlaces] BETWEEN 0 AND 8 AND [QuantityDecimalPlaces] BETWEEN 0 AND 8 AND [ExchangeRateDecimalPlaces] BETWEEN 0 AND 8 AND [PercentageDecimalPlaces] BETWEEN 0 AND 8 AND [CostDecimalPlaces] BETWEEN 0 AND 8");
                    table.CheckConstraint("CK_RII_GENERAL_SETTINGS_LIMITS", "[DefaultPageSize] BETWEEN 10 AND 100 AND [MaximumExportRows] BETWEEN 100 AND 100000");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_GENERAL_SETTINGS_ScopeKey",
                table: "RII_GENERAL_SETTINGS",
                column: "ScopeKey",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_GENERAL_SETTINGS");
        }
    }
}
