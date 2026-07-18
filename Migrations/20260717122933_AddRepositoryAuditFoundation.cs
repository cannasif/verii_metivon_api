using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryAuditFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "RII_USERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RII_USERS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedBy",
                table: "RII_USERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RII_USERS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "RII_USERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "RII_USER_DETAILS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RII_USER_DETAILS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedBy",
                table: "RII_USER_DETAILS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RII_USER_DETAILS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "RII_USER_DETAILS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "RII_BRANCHES",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RII_BRANCHES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedBy",
                table: "RII_BRANCHES",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RII_BRANCHES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "RII_BRANCHES",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RII_USERS");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RII_USERS");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RII_USERS");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RII_USERS");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RII_USERS");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RII_USER_DETAILS");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RII_BRANCHES");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RII_BRANCHES");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RII_BRANCHES");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RII_BRANCHES");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RII_BRANCHES");
        }
    }
}
