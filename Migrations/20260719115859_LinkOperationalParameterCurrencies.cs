using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class LinkOperationalParameterCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InventoryCurrencyId",
                table: "RII_TRANSFER_PARAMETERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InventoryCurrencyId",
                table: "RII_SHIPPING_PARAMETERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InventoryCurrencyId",
                table: "RII_RECEIVING_PARAMETERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PostingCurrencyId",
                table: "RII_INVENTORY_COUNT_PARAMETERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultCurrencyId",
                table: "RII_EDOCUMENT_PARAMETERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("""
                DECLARE @FallbackCurrencyId bigint = (SELECT TOP (1) [Id] FROM [RII_CURRENCIES] ORDER BY CASE WHEN [IsoCode] = 'TRY' OR [Code] = 'TRY' THEN 0 ELSE 1 END, [Id]);
                IF @FallbackCurrencyId IS NULL THROW 51000, 'Currency master data is required before linking module parameters.', 1;

                UPDATE p SET [InventoryCurrencyId] = COALESCE(c.[Id], @FallbackCurrencyId) FROM [RII_TRANSFER_PARAMETERS] p OUTER APPLY (SELECT TOP (1) [Id] FROM [RII_CURRENCIES] WHERE [IsoCode]=p.[InventoryCurrencyCode] OR [Code]=p.[InventoryCurrencyCode] ORDER BY [Id]) c;
                UPDATE p SET [InventoryCurrencyId] = COALESCE(c.[Id], @FallbackCurrencyId) FROM [RII_SHIPPING_PARAMETERS] p OUTER APPLY (SELECT TOP (1) [Id] FROM [RII_CURRENCIES] WHERE [IsoCode]=p.[InventoryCurrencyCode] OR [Code]=p.[InventoryCurrencyCode] ORDER BY [Id]) c;
                UPDATE p SET [InventoryCurrencyId] = COALESCE(c.[Id], @FallbackCurrencyId) FROM [RII_RECEIVING_PARAMETERS] p OUTER APPLY (SELECT TOP (1) [Id] FROM [RII_CURRENCIES] WHERE [IsoCode]=p.[InventoryCurrencyCode] OR [Code]=p.[InventoryCurrencyCode] ORDER BY [Id]) c;
                UPDATE p SET [PostingCurrencyId] = COALESCE(c.[Id], @FallbackCurrencyId) FROM [RII_INVENTORY_COUNT_PARAMETERS] p OUTER APPLY (SELECT TOP (1) [Id] FROM [RII_CURRENCIES] WHERE [IsoCode]=p.[PostingCurrencyCode] OR [Code]=p.[PostingCurrencyCode] ORDER BY [Id]) c;
                UPDATE p SET [DefaultCurrencyId] = COALESCE(c.[Id], @FallbackCurrencyId) FROM [RII_EDOCUMENT_PARAMETERS] p OUTER APPLY (SELECT TOP (1) [Id] FROM [RII_CURRENCIES] WHERE [IsoCode]=p.[DefaultCurrencyCode] OR [Code]=p.[DefaultCurrencyCode] ORDER BY [Id]) c;
                """);

            migrationBuilder.AlterColumn<long>(name:"InventoryCurrencyId",table:"RII_TRANSFER_PARAMETERS",type:"bigint",nullable:false,oldClrType:typeof(long),oldType:"bigint",oldNullable:true);
            migrationBuilder.AlterColumn<long>(name:"InventoryCurrencyId",table:"RII_SHIPPING_PARAMETERS",type:"bigint",nullable:false,oldClrType:typeof(long),oldType:"bigint",oldNullable:true);
            migrationBuilder.AlterColumn<long>(name:"InventoryCurrencyId",table:"RII_RECEIVING_PARAMETERS",type:"bigint",nullable:false,oldClrType:typeof(long),oldType:"bigint",oldNullable:true);
            migrationBuilder.AlterColumn<long>(name:"PostingCurrencyId",table:"RII_INVENTORY_COUNT_PARAMETERS",type:"bigint",nullable:false,oldClrType:typeof(long),oldType:"bigint",oldNullable:true);
            migrationBuilder.AlterColumn<long>(name:"DefaultCurrencyId",table:"RII_EDOCUMENT_PARAMETERS",type:"bigint",nullable:false,oldClrType:typeof(long),oldType:"bigint",oldNullable:true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRANSFER_PARAMETERS_InventoryCurrencyId",
                table: "RII_TRANSFER_PARAMETERS",
                column: "InventoryCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPPING_PARAMETERS_InventoryCurrencyId",
                table: "RII_SHIPPING_PARAMETERS",
                column: "InventoryCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_RECEIVING_PARAMETERS_InventoryCurrencyId",
                table: "RII_RECEIVING_PARAMETERS",
                column: "InventoryCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNT_PARAMETERS_PostingCurrencyId",
                table: "RII_INVENTORY_COUNT_PARAMETERS",
                column: "PostingCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_EDOCUMENT_PARAMETERS_DefaultCurrencyId",
                table: "RII_EDOCUMENT_PARAMETERS",
                column: "DefaultCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RII_EDOCUMENT_PARAMETERS_RII_CURRENCIES_DefaultCurrencyId",
                table: "RII_EDOCUMENT_PARAMETERS",
                column: "DefaultCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_INVENTORY_COUNT_PARAMETERS_RII_CURRENCIES_PostingCurrencyId",
                table: "RII_INVENTORY_COUNT_PARAMETERS",
                column: "PostingCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_RECEIVING_PARAMETERS_RII_CURRENCIES_InventoryCurrencyId",
                table: "RII_RECEIVING_PARAMETERS",
                column: "InventoryCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_SHIPPING_PARAMETERS_RII_CURRENCIES_InventoryCurrencyId",
                table: "RII_SHIPPING_PARAMETERS",
                column: "InventoryCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_TRANSFER_PARAMETERS_RII_CURRENCIES_InventoryCurrencyId",
                table: "RII_TRANSFER_PARAMETERS",
                column: "InventoryCurrencyId",
                principalTable: "RII_CURRENCIES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_EDOCUMENT_PARAMETERS_RII_CURRENCIES_DefaultCurrencyId",
                table: "RII_EDOCUMENT_PARAMETERS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_INVENTORY_COUNT_PARAMETERS_RII_CURRENCIES_PostingCurrencyId",
                table: "RII_INVENTORY_COUNT_PARAMETERS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_RECEIVING_PARAMETERS_RII_CURRENCIES_InventoryCurrencyId",
                table: "RII_RECEIVING_PARAMETERS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_SHIPPING_PARAMETERS_RII_CURRENCIES_InventoryCurrencyId",
                table: "RII_SHIPPING_PARAMETERS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_TRANSFER_PARAMETERS_RII_CURRENCIES_InventoryCurrencyId",
                table: "RII_TRANSFER_PARAMETERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_TRANSFER_PARAMETERS_InventoryCurrencyId",
                table: "RII_TRANSFER_PARAMETERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SHIPPING_PARAMETERS_InventoryCurrencyId",
                table: "RII_SHIPPING_PARAMETERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_RECEIVING_PARAMETERS_InventoryCurrencyId",
                table: "RII_RECEIVING_PARAMETERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_COUNT_PARAMETERS_PostingCurrencyId",
                table: "RII_INVENTORY_COUNT_PARAMETERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_EDOCUMENT_PARAMETERS_DefaultCurrencyId",
                table: "RII_EDOCUMENT_PARAMETERS");

            migrationBuilder.DropColumn(
                name: "InventoryCurrencyId",
                table: "RII_TRANSFER_PARAMETERS");

            migrationBuilder.DropColumn(
                name: "InventoryCurrencyId",
                table: "RII_SHIPPING_PARAMETERS");

            migrationBuilder.DropColumn(
                name: "InventoryCurrencyId",
                table: "RII_RECEIVING_PARAMETERS");

            migrationBuilder.DropColumn(
                name: "PostingCurrencyId",
                table: "RII_INVENTORY_COUNT_PARAMETERS");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyId",
                table: "RII_EDOCUMENT_PARAMETERS");
        }
    }
}
