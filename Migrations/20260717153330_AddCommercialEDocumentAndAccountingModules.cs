using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace verii_metivon_api.Migrations
{
    /// <inheritdoc />
    public partial class AddCommercialEDocumentAndAccountingModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RII_COMPANY_LEGAL_PROFILES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    TaxOffice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MersisNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeRegistryNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEInvoiceRegistered = table.Column<bool>(type: "bit", nullable: false),
                    IsEDespatchRegistered = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_COMPANY_LEGAL_PROFILES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_DISCOUNT_RULES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    BusinessPartnerId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerGroupId = table.Column<long>(type: "bigint", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: true),
                    ProductGroupId = table.Column<long>(type: "bigint", nullable: true),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsExclusive = table.Column<bool>(type: "bit", nullable: false),
                    MaximumDiscountAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_DISCOUNT_RULES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    Environment = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InvoiceServiceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DespatchServiceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CredentialReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    MaximumRetryCount = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_EDOCUMENT_PROVIDER_CONFIGURATIONS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_ELECTRONIC_DOCUMENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scenario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderDocumentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnvelopeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayloadXml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayloadHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_RII_ELECTRONIC_DOCUMENTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_FISCAL_PERIODS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsOpen = table.Column<bool>(type: "bit", nullable: false),
                    IsInventoryClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsGeneralLedgerClosed = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_RII_FISCAL_PERIODS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_COUNTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: true),
                    CountDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsBlindCount = table.Column<bool>(type: "bit", nullable: false),
                    FrozenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InventoryPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_RII_INVENTORY_COUNTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_COUNTS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_LEDGER_ACCOUNTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    AllowPosting = table.Column<bool>(type: "bit", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_RII_LEDGER_ACCOUNTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_LEDGER_ACCOUNTS_RII_LEDGER_ACCOUNTS_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RII_LEDGER_ACCOUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRICE_LISTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
                    PricesIncludeTax = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RII_PRICE_LISTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PRICE_LISTS_RII_CURRENCIES_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "RII_CURRENCIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_SALES_ORDERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentTermId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedShipmentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CustomerReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_RII_SALES_ORDERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_SALES_ORDERS_RII_BUSINESS_PARTNERS_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "RII_BUSINESS_PARTNERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_SALES_ORDERS_RII_CURRENCIES_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "RII_CURRENCIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_SALES_ORDERS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_ELECTRONIC_DOCUMENT_STATUS_HISTORY",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectronicDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ProviderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_RII_ELECTRONIC_DOCUMENT_STATUS_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_ELECTRONIC_DOCUMENT_STATUS_HISTORY_RII_ELECTRONIC_DOCUMENTS_ElectronicDocumentId",
                        column: x => x.ElectronicDocumentId,
                        principalTable: "RII_ELECTRONIC_DOCUMENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_COST_CLOSES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CloseNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FiscalPeriodId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionCount = table.Column<int>(type: "int", nullable: false),
                    AdjustmentAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_INVENTORY_COST_CLOSES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_COST_CLOSES_RII_FISCAL_PERIODS_FiscalPeriodId",
                        column: x => x.FiscalPeriodId,
                        principalTable: "RII_FISCAL_PERIODS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_JOURNAL_ENTRIES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FiscalPeriodId = table.Column<long>(type: "bigint", nullable: false),
                    PostingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceId = table.Column<long>(type: "bigint", nullable: false),
                    SourceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReversalOfId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_RII_JOURNAL_ENTRIES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_JOURNAL_ENTRIES_RII_FISCAL_PERIODS_FiscalPeriodId",
                        column: x => x.FiscalPeriodId,
                        principalTable: "RII_FISCAL_PERIODS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_JOURNAL_ENTRIES_RII_JOURNAL_ENTRIES_ReversalOfId",
                        column: x => x.ReversalOfId,
                        principalTable: "RII_JOURNAL_ENTRIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_COUNT_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryCountId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    SystemQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: true),
                    DifferenceQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_INVENTORY_COUNT_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_COUNT_LINES_RII_INVENTORY_COUNTS_InventoryCountId",
                        column: x => x.InventoryCountId,
                        principalTable: "RII_INVENTORY_COUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_INVENTORY_POSTING_PROFILES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    DebitAccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreditAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ProductGroupId = table.Column<long>(type: "bigint", nullable: true),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_RII_INVENTORY_POSTING_PROFILES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_POSTING_PROFILES_RII_LEDGER_ACCOUNTS_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "RII_LEDGER_ACCOUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_INVENTORY_POSTING_PROFILES_RII_LEDGER_ACCOUNTS_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "RII_LEDGER_ACCOUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_PRICE_LIST_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerGroupId = table.Column<long>(type: "bigint", nullable: true),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_RII_PRICE_LIST_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_PRICE_LIST_LINES_RII_CUSTOMER_GROUPS_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalTable: "RII_CUSTOMER_GROUPS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RII_PRICE_LIST_LINES_RII_PRICE_LISTS_PriceListId",
                        column: x => x.PriceListId,
                        principalTable: "RII_PRICE_LISTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_PRICE_LIST_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_PRICE_LIST_LINES_RII_UNITS_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RII_UNITS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_SALES_ORDER_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    PriceListLineId = table.Column<long>(type: "bigint", nullable: true),
                    DiscountRuleId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_RII_SALES_ORDER_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_SALES_ORDER_LINES_RII_PRODUCTS_ProductId",
                        column: x => x.ProductId,
                        principalTable: "RII_PRODUCTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_SALES_ORDER_LINES_RII_SALES_ORDERS_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "RII_SALES_ORDERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_SHIPMENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    WarehouseId = table.Column<long>(type: "bigint", nullable: false),
                    ShipmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CarrierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehiclePlate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InventoryPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_RII_SHIPMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_SHIPMENTS_RII_SALES_ORDERS_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "RII_SALES_ORDERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_SHIPMENTS_RII_WAREHOUSES_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "RII_WAREHOUSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_JOURNAL_ENTRY_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    LedgerAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ForeignDebit = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    ForeignCredit = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
                    BusinessPartnerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_JOURNAL_ENTRY_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_JOURNAL_ENTRY_LINES_RII_JOURNAL_ENTRIES_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "RII_JOURNAL_ENTRIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RII_JOURNAL_ENTRY_LINES_RII_LEDGER_ACCOUNTS_LedgerAccountId",
                        column: x => x.LedgerAccountId,
                        principalTable: "RII_LEDGER_ACCOUNTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RII_DELIVERY_NOTES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryNoteNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ShipmentId = table.Column<long>(type: "bigint", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scenario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GibUuid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EnvelopeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RII_DELIVERY_NOTES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_DELIVERY_NOTES_RII_SHIPMENTS_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "RII_SHIPMENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RII_SHIPMENT_LINES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<long>(type: "bigint", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    SalesOrderLineId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    StorageLocationId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryStatusId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryLotId = table.Column<long>(type: "bigint", nullable: true),
                    InventorySerialId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(24,8)", precision: 24, scale: 8, nullable: false),
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
                    table.PrimaryKey("PK_RII_SHIPMENT_LINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RII_SHIPMENT_LINES_RII_SALES_ORDER_LINES_SalesOrderLineId",
                        column: x => x.SalesOrderLineId,
                        principalTable: "RII_SALES_ORDER_LINES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RII_SHIPMENT_LINES_RII_SHIPMENTS_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "RII_SHIPMENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RII_COMPANY_LEGAL_PROFILES_BranchId",
                table: "RII_COMPANY_LEGAL_PROFILES",
                column: "BranchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_DeliveryNoteNumber",
                table: "RII_DELIVERY_NOTES",
                column: "DeliveryNoteNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_GibUuid",
                table: "RII_DELIVERY_NOTES",
                column: "GibUuid",
                unique: true,
                filter: "[GibUuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_DELIVERY_NOTES_ShipmentId",
                table: "RII_DELIVERY_NOTES",
                column: "ShipmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_DISCOUNT_RULES_Code",
                table: "RII_DISCOUNT_RULES",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_EDOCUMENT_PROVIDER_CONFIGURATIONS_BranchId_ProviderType_Environment",
                table: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS",
                columns: new[] { "BranchId", "ProviderType", "Environment" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENT_STATUS_HISTORY_ElectronicDocumentId_OccurredAt",
                table: "RII_ELECTRONIC_DOCUMENT_STATUS_HISTORY",
                columns: new[] { "ElectronicDocumentId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_DocumentType_DocumentNumber",
                table: "RII_ELECTRONIC_DOCUMENTS",
                columns: new[] { "DocumentType", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_SourceType_SourceId_DocumentType",
                table: "RII_ELECTRONIC_DOCUMENTS",
                columns: new[] { "SourceType", "SourceId", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_ELECTRONIC_DOCUMENTS_Uuid",
                table: "RII_ELECTRONIC_DOCUMENTS",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_FISCAL_PERIODS_Code",
                table: "RII_FISCAL_PERIODS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COST_CLOSES_CloseNumber",
                table: "RII_INVENTORY_COST_CLOSES",
                column: "CloseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COST_CLOSES_FiscalPeriodId",
                table: "RII_INVENTORY_COST_CLOSES",
                column: "FiscalPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNT_LINES_InventoryCountId_LineNumber",
                table: "RII_INVENTORY_COUNT_LINES",
                columns: new[] { "InventoryCountId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNTS_CountNumber",
                table: "RII_INVENTORY_COUNTS",
                column: "CountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_COUNTS_WarehouseId",
                table: "RII_INVENTORY_COUNTS",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_Code_MovementType",
                table: "RII_INVENTORY_POSTING_PROFILES",
                columns: new[] { "Code", "MovementType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_CreditAccountId",
                table: "RII_INVENTORY_POSTING_PROFILES",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_INVENTORY_POSTING_PROFILES_DebitAccountId",
                table: "RII_INVENTORY_POSTING_PROFILES",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_FiscalPeriodId",
                table: "RII_JOURNAL_ENTRIES",
                column: "FiscalPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_JournalNumber",
                table: "RII_JOURNAL_ENTRIES",
                column: "JournalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_ReversalOfId",
                table: "RII_JOURNAL_ENTRIES",
                column: "ReversalOfId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRIES_SourceType_SourceId",
                table: "RII_JOURNAL_ENTRIES",
                columns: new[] { "SourceType", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRY_LINES_JournalEntryId_LineNumber",
                table: "RII_JOURNAL_ENTRY_LINES",
                columns: new[] { "JournalEntryId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_JOURNAL_ENTRY_LINES_LedgerAccountId",
                table: "RII_JOURNAL_ENTRY_LINES",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_Code",
                table: "RII_LEDGER_ACCOUNTS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_LEDGER_ACCOUNTS_ParentId",
                table: "RII_LEDGER_ACCOUNTS",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LIST_LINES_CustomerGroupId",
                table: "RII_PRICE_LIST_LINES",
                column: "CustomerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LIST_LINES_PriceListId_ProductId_UnitId_CustomerGroupId_MinimumQuantity_ValidFrom",
                table: "RII_PRICE_LIST_LINES",
                columns: new[] { "PriceListId", "ProductId", "UnitId", "CustomerGroupId", "MinimumQuantity", "ValidFrom" },
                unique: true,
                filter: "[CustomerGroupId] IS NOT NULL AND [ValidFrom] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LIST_LINES_ProductId",
                table: "RII_PRICE_LIST_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LIST_LINES_UnitId",
                table: "RII_PRICE_LIST_LINES",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LISTS_Code",
                table: "RII_PRICE_LISTS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_PRICE_LISTS_CurrencyId",
                table: "RII_PRICE_LISTS",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDER_LINES_ProductId",
                table: "RII_SALES_ORDER_LINES",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDER_LINES_SalesOrderId_LineNumber",
                table: "RII_SALES_ORDER_LINES",
                columns: new[] { "SalesOrderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDERS_CurrencyId",
                table: "RII_SALES_ORDERS",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDERS_CustomerId",
                table: "RII_SALES_ORDERS",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDERS_OrderNumber",
                table: "RII_SALES_ORDERS",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SALES_ORDERS_WarehouseId",
                table: "RII_SALES_ORDERS",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENT_LINES_SalesOrderLineId",
                table: "RII_SHIPMENT_LINES",
                column: "SalesOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENT_LINES_ShipmentId_LineNumber",
                table: "RII_SHIPMENT_LINES",
                columns: new[] { "ShipmentId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENTS_SalesOrderId",
                table: "RII_SHIPMENTS",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENTS_ShipmentNumber",
                table: "RII_SHIPMENTS",
                column: "ShipmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RII_SHIPMENTS_WarehouseId",
                table: "RII_SHIPMENTS",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RII_COMPANY_LEGAL_PROFILES");

            migrationBuilder.DropTable(
                name: "RII_DELIVERY_NOTES");

            migrationBuilder.DropTable(
                name: "RII_DISCOUNT_RULES");

            migrationBuilder.DropTable(
                name: "RII_EDOCUMENT_PROVIDER_CONFIGURATIONS");

            migrationBuilder.DropTable(
                name: "RII_ELECTRONIC_DOCUMENT_STATUS_HISTORY");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_COST_CLOSES");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_COUNT_LINES");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_POSTING_PROFILES");

            migrationBuilder.DropTable(
                name: "RII_JOURNAL_ENTRY_LINES");

            migrationBuilder.DropTable(
                name: "RII_PRICE_LIST_LINES");

            migrationBuilder.DropTable(
                name: "RII_SHIPMENT_LINES");

            migrationBuilder.DropTable(
                name: "RII_ELECTRONIC_DOCUMENTS");

            migrationBuilder.DropTable(
                name: "RII_INVENTORY_COUNTS");

            migrationBuilder.DropTable(
                name: "RII_JOURNAL_ENTRIES");

            migrationBuilder.DropTable(
                name: "RII_LEDGER_ACCOUNTS");

            migrationBuilder.DropTable(
                name: "RII_PRICE_LISTS");

            migrationBuilder.DropTable(
                name: "RII_SALES_ORDER_LINES");

            migrationBuilder.DropTable(
                name: "RII_SHIPMENTS");

            migrationBuilder.DropTable(
                name: "RII_FISCAL_PERIODS");

            migrationBuilder.DropTable(
                name: "RII_SALES_ORDERS");
        }
    }
}
