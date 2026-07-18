using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeniorNumberSeriesAndSerialWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_DOCUMENT_NUMBER_SERIES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    Format = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ResetPeriod = table.Column<int>(type: "int", nullable: false),
                    StartingNumber = table.Column<long>(type: "bigint", nullable: false),
                    IncrementBy = table.Column<int>(type: "int", nullable: false),
                    MaximumNumber = table.Column<long>(type: "bigint", nullable: false),
                    IsGibCompliant = table.Column<bool>(type: "bit", nullable: false),
                    AllowManual = table.Column<bool>(type: "bit", nullable: false),
                    IsContinuous = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_DOCUMENT_NUMBER_SERIES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_NUMBER_SERIES_ASSIGNMENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberSeriesId = table.Column<long>(type: "bigint", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    BusinessPartnerId = table.Column<long>(type: "bigint", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Scenario = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_NUMBER_SERIES_ASSIGNMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_NUMBER_SERIES_ASSIGNMENTS_RII_DOCUMENT_NUMBER_SERIES_NumberSeriesId",
                        column: x => x.NumberSeriesId,
                        principalTable: "RII_DOCUMENT_NUMBER_SERIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_NUMBER_SERIES_COUNTERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberSeriesId = table.Column<long>(type: "bigint", nullable: false),
                    PeriodKey = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NextNumber = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RII_NUMBER_SERIES_COUNTERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_NUMBER_SERIES_COUNTERS_RII_DOCUMENT_NUMBER_SERIES_NumberSeriesId",
                        column: x => x.NumberSeriesId,
                        principalTable: "RII_DOCUMENT_NUMBER_SERIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_NUMBER_SERIES_USAGES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberSeriesId = table.Column<long>(type: "bigint", nullable: false),
                    NumberSeriesCounterId = table.Column<long>(type: "bigint", nullable: false),
                    PeriodKey = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_RII_NUMBER_SERIES_USAGES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_NUMBER_SERIES_USAGES_RII_DOCUMENT_NUMBER_SERIES_NumberSeriesId",
                        column: x => x.NumberSeriesId,
                        principalTable: "RII_DOCUMENT_NUMBER_SERIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_NUMBER_SERIES_USAGES_RII_NUMBER_SERIES_COUNTERS_NumberSeriesCounterId",
                        column: x => x.NumberSeriesCounterId,
                        principalTable: "RII_NUMBER_SERIES_COUNTERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_DOCUMENT_NUMBER_SERIES_Code",
                table: "RII_DOCUMENT_NUMBER_SERIES",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DOCUMENT_NUMBER_SERIES_Module_Reference_BranchId_WarehouseId_Priority",
                table: "RII_DOCUMENT_NUMBER_SERIES",
                columns: new[] { "Module", "Reference", "BranchId", "WarehouseId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_ASSIGNMENTS_NumberSeriesId_BranchId_WarehouseId_UserId_BusinessPartnerId_Channel_Scenario",
                table: "RII_NUMBER_SERIES_ASSIGNMENTS",
                columns: new[] { "NumberSeriesId", "BranchId", "WarehouseId", "UserId", "BusinessPartnerId", "Channel", "Scenario" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_COUNTERS_NumberSeriesId_PeriodKey",
                table: "RII_NUMBER_SERIES_COUNTERS",
                columns: new[] { "NumberSeriesId", "PeriodKey" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_DocumentNumber",
                table: "RII_NUMBER_SERIES_USAGES",
                column: "DocumentNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_DocumentType_DocumentId",
                table: "RII_NUMBER_SERIES_USAGES",
                columns: new[] { "DocumentType", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_NumberSeriesCounterId",
                table: "RII_NUMBER_SERIES_USAGES",
                column: "NumberSeriesCounterId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_NUMBER_SERIES_USAGES_NumberSeriesId",
                table: "RII_NUMBER_SERIES_USAGES",
                column: "NumberSeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_NUMBER_SERIES_ASSIGNMENTS");

            migrationBuilder.DropTable(
                name: "RII_NUMBER_SERIES_USAGES");

            migrationBuilder.DropTable(
                name: "RII_NUMBER_SERIES_COUNTERS");

            migrationBuilder.DropTable(
                name: "RII_DOCUMENT_NUMBER_SERIES");
        }
    }
}
