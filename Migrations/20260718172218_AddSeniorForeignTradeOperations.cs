using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeniorForeignTradeOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TradeDossierId",
                table: "RII_SHIPMENTS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TradeDossierId",
                table: "RII_IMPORT_DOSSIERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TradeDossierId",
                table: "RII_GOODS_RECEIPTS",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RII_TRADE_ATTACHMENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeDossierId = table.Column<long>(type: "bigint", nullable: false),
                    CustomsDeclarationId = table.Column<long>(type: "bigint", nullable: true),
                    AttachmentType = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DocumentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_TRADE_ATTACHMENTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_TRADE_DOSSIERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessPartnerId = table.Column<long>(type: "bigint", nullable: false),
                    CustomsBrokerId = table.Column<long>(type: "bigint", nullable: true),
                    CarrierId = table.Column<long>(type: "bigint", nullable: true),
                    CurrencyCode = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    IncotermCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    PaymentMethodCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DeliveryMethodCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CustomsOfficeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BondedWarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    OpenDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EstimatedArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ActualArrivalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_RII_TRADE_DOSSIERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_TRADE_DOSSIERS_RII_BUSINESS_PARTNERS_BusinessPartnerId",
                        column: x => x.BusinessPartnerId,
                        principalTable: "RII_BUSINESS_PARTNERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_CUSTOMS_DECLARATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeDossierId = table.Column<long>(type: "bigint", nullable: false),
                    DeclarationType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeclarationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DeclarationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RegimeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CustomsOfficeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BondedWarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    CustomsExchangeRate = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    GuaranteeReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_CUSTOMS_DECLARATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_CUSTOMS_DECLARATIONS_RII_TRADE_DOSSIERS_TradeDossierId",
                        column: x => x.TradeDossierId,
                        principalTable: "RII_TRADE_DOSSIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_TRADE_DOCUMENT_LINKS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeDossierId = table.Column<long>(type: "bigint", nullable: false),
                    LinkType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: false),
                    SourceLineId = table.Column<long>(type: "bigint", nullable: true),
                    LinkedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
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
                    table.PrimaryKey("PK_RII_TRADE_DOCUMENT_LINKS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_TRADE_DOCUMENT_LINKS_RII_TRADE_DOSSIERS_TradeDossierId",
                        column: x => x.TradeDossierId,
                        principalTable: "RII_TRADE_DOSSIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_TRADE_DOSSIER_STATUS_HISTORY",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeDossierId = table.Column<long>(type: "bigint", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponsiblePartnerId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_RII_TRADE_DOSSIER_STATUS_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_TRADE_DOSSIER_STATUS_HISTORY_RII_TRADE_DOSSIERS_TradeDossierId",
                        column: x => x.TradeDossierId,
                        principalTable: "RII_TRADE_DOSSIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_CUSTOMS_DECLARATION_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomsDeclarationId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    GtipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryOfOriginCode = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    NetWeight = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    PackageCount = table.Column<int>(type: "int", nullable: false),
                    CustomsValue = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    CustomsDuty = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReleasedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
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
                    table.PrimaryKey("PK_RII_CUSTOMS_DECLARATION_LINES", x => x.Id);
                    table.CheckConstraint("CK_RII_CUSTOMS_DECLARATION_LINES_RELEASED", "[ReleasedQuantity] >= 0 AND [ReleasedQuantity] <= [Quantity]");
                    table.ForeignKey(
                        name: "FK_RII_CUSTOMS_DECLARATION_LINES_RII_CUSTOMS_DECLARATIONS_CustomsDeclarationId",
                        column: x => x.CustomsDeclarationId,
                        principalTable: "RII_CUSTOMS_DECLARATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_CUSTOMS_DECLARATION_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENTS_TradeDossierId",
                table: "RII_SHIPMENTS",
                column: "TradeDossierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_GOODS_RECEIPTS_TradeDossierId",
                table: "RII_GOODS_RECEIPTS",
                column: "TradeDossierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_LINES_CustomsDeclarationId_LineNumber",
                table: "RII_CUSTOMS_DECLARATION_LINES",
                columns: new[] { "CustomsDeclarationId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATION_LINES_ProductId",
                table: "RII_CUSTOMS_DECLARATION_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATIONS_DeclarationNumber",
                table: "RII_CUSTOMS_DECLARATIONS",
                column: "DeclarationNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_CUSTOMS_DECLARATIONS_TradeDossierId",
                table: "RII_CUSTOMS_DECLARATIONS",
                column: "TradeDossierId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_ATTACHMENTS_TradeDossierId_Sha256",
                table: "RII_TRADE_ATTACHMENTS",
                columns: new[] { "TradeDossierId", "Sha256" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_DOCUMENT_LINKS_TradeDossierId_LinkType_SourceId_SourceLineId",
                table: "RII_TRADE_DOCUMENT_LINKS",
                columns: new[] { "TradeDossierId", "LinkType", "SourceId", "SourceLineId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_DOSSIER_STATUS_HISTORY_TradeDossierId_OccurredAt",
                table: "RII_TRADE_DOSSIER_STATUS_HISTORY",
                columns: new[] { "TradeDossierId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_DOSSIERS_BusinessPartnerId",
                table: "RII_TRADE_DOSSIERS",
                column: "BusinessPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_DOSSIERS_Direction_Status_BranchId",
                table: "RII_TRADE_DOSSIERS",
                columns: new[] { "Direction", "Status", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_TRADE_DOSSIERS_DossierNumber",
                table: "RII_TRADE_DOSSIERS",
                column: "DossierNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_RII_GOODS_RECEIPTS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_GOODS_RECEIPTS",
                column: "TradeDossierId",
                principalTable: "RII_TRADE_DOSSIERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RII_SHIPMENTS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_SHIPMENTS",
                column: "TradeDossierId",
                principalTable: "RII_TRADE_DOSSIERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RII_GOODS_RECEIPTS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_GOODS_RECEIPTS");

            migrationBuilder.DropForeignKey(
                name: "FK_RII_SHIPMENTS_RII_TRADE_DOSSIERS_TradeDossierId",
                table: "RII_SHIPMENTS");

            migrationBuilder.DropTable(
                name: "RII_CUSTOMS_DECLARATION_LINES");

            migrationBuilder.DropTable(
                name: "RII_TRADE_ATTACHMENTS");

            migrationBuilder.DropTable(
                name: "RII_TRADE_DOCUMENT_LINKS");

            migrationBuilder.DropTable(
                name: "RII_TRADE_DOSSIER_STATUS_HISTORY");

            migrationBuilder.DropTable(
                name: "RII_CUSTOMS_DECLARATIONS");

            migrationBuilder.DropTable(
                name: "RII_TRADE_DOSSIERS");

            migrationBuilder.DropIndex(
                name: "IX_RII_SHIPMENTS_TradeDossierId",
                table: "RII_SHIPMENTS");

            migrationBuilder.DropIndex(
                name: "IX_RII_GOODS_RECEIPTS_TradeDossierId",
                table: "RII_GOODS_RECEIPTS");

            migrationBuilder.DropColumn(
                name: "TradeDossierId",
                table: "RII_SHIPMENTS");

            migrationBuilder.DropColumn(
                name: "TradeDossierId",
                table: "RII_IMPORT_DOSSIERS");

            migrationBuilder.DropColumn(
                name: "TradeDossierId",
                table: "RII_GOODS_RECEIPTS");
        }
    }
}
