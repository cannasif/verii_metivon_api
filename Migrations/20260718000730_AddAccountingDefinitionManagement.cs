using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingDefinitionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_Code_MovementType",
                table: "RII_INVENTORY_POSTING_PROFILES");

            migrationBuilder.DropIndex(
                name: "IX_RII_FISCAL_PERIODS_Code",
                table: "RII_FISCAL_PERIODS");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RII_INVENTORY_POSTING_PROFILES",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "RII_INVENTORY_POSTING_PROFILES",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RII_FISCAL_PERIODS",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "RII_FISCAL_PERIODS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_Code_MovementType",
                table: "RII_INVENTORY_POSTING_PROFILES",
                columns: new[] { "Code", "MovementType" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_FISCAL_PERIODS_Code",
                table: "RII_FISCAL_PERIODS",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_Code_MovementType",
                table: "RII_INVENTORY_POSTING_PROFILES");

            migrationBuilder.DropIndex(
                name: "IX_RII_FISCAL_PERIODS_Code",
                table: "RII_FISCAL_PERIODS");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RII_INVENTORY_POSTING_PROFILES",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "RII_INVENTORY_POSTING_PROFILES",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RII_FISCAL_PERIODS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "RII_FISCAL_PERIODS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_Code_MovementType",
                table: "RII_INVENTORY_POSTING_PROFILES",
                columns: new[] { "Code", "MovementType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_FISCAL_PERIODS_Code",
                table: "RII_FISCAL_PERIODS",
                column: "Code",
                unique: true);
        }
    }
}
