using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Warehouses.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Entities;
using verii_metivon_api.Modules.Procurement.Domain.Entities;
using verii_metivon_api.Modules.Receiving.Domain.Entities;
using verii_metivon_api.Modules.Transfers.Domain.Entities;
using verii_metivon_api.Modules.Pricing.Domain.Entities;
using verii_metivon_api.Modules.Sales.Domain.Entities;
using verii_metivon_api.Modules.Shipping.Domain.Entities;
using verii_metivon_api.Modules.Counting.Domain.Entities;
using verii_metivon_api.Modules.EDocuments.Domain.Entities;
using verii_metivon_api.Modules.Accounting.Domain.Entities;
using verii_metivon_api.Modules.LandedCosts.Domain.Entities;
using verii_metivon_api.Modules.Parameters.Domain.Entities;
using verii_metivon_api.Modules.NumberSeries.Domain.Entities;
using verii_metivon_api.Modules.TradeOperations.Domain.Entities;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Core.Persistence;

public sealed class MetivonDbContext(DbContextOptions<MetivonDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserDetail> UserDetails => Set<UserDetail>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();
    public DbSet<BusinessPartnerType> BusinessPartnerTypes => Set<BusinessPartnerType>();
    public DbSet<CustomerGroup> CustomerGroups => Set<CustomerGroup>();
    public DbSet<PaymentTerm> PaymentTerms => Set<PaymentTerm>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<TaxGroup> TaxGroups => Set<TaxGroup>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<UnitCategory> UnitCategories => Set<UnitCategory>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<PackageType> PackageTypes => Set<PackageType>();
    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
    public DbSet<ProductBarcode> ProductBarcodes => Set<ProductBarcode>();
    public DbSet<ProductPackaging> ProductPackagings => Set<ProductPackaging>();
    public DbSet<ProductBranchSetting> ProductBranchSettings => Set<ProductBranchSetting>();
    public DbSet<ProductTranslation> ProductTranslations => Set<ProductTranslation>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseType> WarehouseTypes => Set<WarehouseType>();
    public DbSet<LocationType> LocationTypes => Set<LocationType>();
    public DbSet<WarehouseZone> WarehouseZones => Set<WarehouseZone>();
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
    public DbSet<InventoryStatus> InventoryStatuses => Set<InventoryStatus>();
    public DbSet<InventoryLot> InventoryLots => Set<InventoryLot>();
    public DbSet<InventorySerial> InventorySerials => Set<InventorySerial>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();
    public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();
    public DbSet<InventoryCostLayer> InventoryCostLayers => Set<InventoryCostLayer>();
    public DbSet<InventoryTraceabilityParameterSettings> InventoryTraceabilityParameterSettings => Set<InventoryTraceabilityParameterSettings>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<ProcurementParameterSettings> ProcurementParameterSettings => Set<ProcurementParameterSettings>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptLine> GoodsReceiptLines => Set<GoodsReceiptLine>();
    public DbSet<GoodsReceiptSerial> GoodsReceiptSerials => Set<GoodsReceiptSerial>();
    public DbSet<TransferOrder> TransferOrders => Set<TransferOrder>();
    public DbSet<TransferOrderLine> TransferOrderLines => Set<TransferOrderLine>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListLine> PriceListLines => Set<PriceListLine>();
    public DbSet<DiscountRule> DiscountRules => Set<DiscountRule>();
    public DbSet<PricingParameterSettings> PricingParameterSettings => Set<PricingParameterSettings>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
    public DbSet<SalesOrderParameterSettings> SalesOrderParameterSettings => Set<SalesOrderParameterSettings>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentLine> ShipmentLines => Set<ShipmentLine>();
    public DbSet<DeliveryNote> DeliveryNotes => Set<DeliveryNote>();
    public DbSet<InventoryCount> InventoryCounts => Set<InventoryCount>();
    public DbSet<InventoryCountLine> InventoryCountLines => Set<InventoryCountLine>();
    public DbSet<InventoryCountParameterSettings> InventoryCountParameterSettings => Set<InventoryCountParameterSettings>();
    public DbSet<CompanyLegalProfile> CompanyLegalProfiles => Set<CompanyLegalProfile>();
    public DbSet<EDocumentProviderConfiguration> EDocumentProviderConfigurations => Set<EDocumentProviderConfiguration>();
    public DbSet<ElectronicDocument> ElectronicDocuments => Set<ElectronicDocument>();
    public DbSet<ElectronicDocumentStatusHistory> ElectronicDocumentStatusHistory => Set<ElectronicDocumentStatusHistory>();
    public DbSet<EDocumentParameterSettings> EDocumentParameterSettings => Set<EDocumentParameterSettings>();
    public DbSet<LedgerAccount> LedgerAccounts => Set<LedgerAccount>();
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
    public DbSet<InventoryPostingProfile> InventoryPostingProfiles => Set<InventoryPostingProfile>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<InventoryCostClose> InventoryCostCloses => Set<InventoryCostClose>();
    public DbSet<AccountingParameterSettings> AccountingParameterSettings => Set<AccountingParameterSettings>();
    public DbSet<LandedCostType> LandedCostTypes => Set<LandedCostType>();
    public DbSet<ImportDossier> ImportDossiers => Set<ImportDossier>();
    public DbSet<ImportDossierLine> ImportDossierLines => Set<ImportDossierLine>();
    public DbSet<ImportDossierCost> ImportDossierCosts => Set<ImportDossierCost>();
    public DbSet<ImportDossierAllocation> ImportDossierAllocations => Set<ImportDossierAllocation>();
    public DbSet<ImportDossierDocument> ImportDossierDocuments => Set<ImportDossierDocument>();
    public DbSet<ErpParameter> ErpParameters => Set<ErpParameter>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<ProductParameterSettings> ProductParameterSettings => Set<ProductParameterSettings>();
    public DbSet<WarehouseParameterSettings> WarehouseParameterSettings => Set<WarehouseParameterSettings>();
    public DbSet<ReceivingParameterSettings> ReceivingParameterSettings => Set<ReceivingParameterSettings>();
    public DbSet<TransferParameterSettings> TransferParameterSettings => Set<TransferParameterSettings>();
    public DbSet<ShippingParameterSettings> ShippingParameterSettings => Set<ShippingParameterSettings>();
    public DbSet<DocumentNumberSeries> DocumentNumberSeries => Set<DocumentNumberSeries>();
    public DbSet<NumberSeriesAssignment> NumberSeriesAssignments => Set<NumberSeriesAssignment>();
    public DbSet<NumberSeriesCounter> NumberSeriesCounters => Set<NumberSeriesCounter>();
    public DbSet<NumberSeriesUsage> NumberSeriesUsages => Set<NumberSeriesUsage>();
    public DbSet<TradeDossier> TradeDossiers => Set<TradeDossier>();
    public DbSet<CustomsDeclaration> CustomsDeclarations => Set<CustomsDeclaration>();
    public DbSet<CustomsDeclarationLine> CustomsDeclarationLines => Set<CustomsDeclarationLine>();
    public DbSet<TradeDossierStatusHistory> TradeDossierStatusHistory => Set<TradeDossierStatusHistory>();
    public DbSet<CustomsDeclarationStatusHistory> CustomsDeclarationStatusHistory => Set<CustomsDeclarationStatusHistory>();
    public DbSet<TradeDocumentLink> TradeDocumentLinks => Set<TradeDocumentLink>();
    public DbSet<TradeAttachment> TradeAttachments => Set<TradeAttachment>();
    public DbSet<PermissionDefinition> PermissionDefinitions => Set<PermissionDefinition>();
    public DbSet<PermissionGroup> PermissionGroups => Set<PermissionGroup>();
    public DbSet<PermissionGroupPermission> PermissionGroupPermissions => Set<PermissionGroupPermission>();
    public DbSet<UserPermissionGroup> UserPermissionGroups => Set<UserPermissionGroup>();
    public DbSet<VisibilityPolicy> VisibilityPolicies => Set<VisibilityPolicy>();
    public DbSet<UserVisibilityPolicy> UserVisibilityPolicies => Set<UserVisibilityPolicy>();
    public DbSet<AccessControlAuditLog> AccessControlAuditLogs => Set<AccessControlAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MetivonDbContext).Assembly);

        ConfigureDefinition(modelBuilder.Entity<BusinessPartnerType>(), "RII_BUSINESS_PARTNER_TYPES");
        ConfigureDefinition(modelBuilder.Entity<CustomerGroup>(), "RII_CUSTOMER_GROUPS");
        ConfigureDefinition(modelBuilder.Entity<PaymentTerm>(), "RII_PAYMENT_TERMS");
        ConfigureDefinition(modelBuilder.Entity<Currency>(), "RII_CURRENCIES");
        ConfigureDefinition(modelBuilder.Entity<TaxGroup>(), "RII_TAX_GROUPS");

        modelBuilder.Entity<PaymentTerm>(entity =>
        {
            entity.Property(x => x.DiscountRate).HasPrecision(7, 4);
        });
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.Property(x => x.IsoCode).HasMaxLength(3).IsRequired();
            entity.Property(x => x.Symbol).HasMaxLength(10).IsRequired();
            entity.HasIndex(x => x.IsoCode).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        modelBuilder.Entity<BusinessPartner>(entity =>
        {
            entity.ToTable("RII_BUSINESS_PARTNERS");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.LegalName).HasMaxLength(250);
            entity.Property(x => x.TaxOffice).HasMaxLength(100);
            entity.Property(x => x.TaxNumber).HasMaxLength(20);
            entity.Property(x => x.NationalIdentityNumber).HasMaxLength(11);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.MobilePhone).HasMaxLength(30);
            entity.Property(x => x.Website).HasMaxLength(250);
            entity.Property(x => x.CreditLimit).HasPrecision(18, 2);
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
            entity.HasIndex(x => x.TaxNumber).HasFilter("[TaxNumber] IS NOT NULL AND [IsDeleted] = 0");
            entity.HasIndex(x => x.NationalIdentityNumber).HasFilter("[NationalIdentityNumber] IS NOT NULL AND [IsDeleted] = 0");
            entity.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.BusinessPartnerType).WithMany().HasForeignKey(x => x.BusinessPartnerTypeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CustomerGroup).WithMany().HasForeignKey(x => x.CustomerGroupId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.PaymentTerm).WithMany().HasForeignKey(x => x.PaymentTermId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.TaxGroup).WithMany().HasForeignKey(x => x.TaxGroupId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }

    private static void ConfigureDefinition<T>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity, string tableName)
        where T : DefinitionEntity
    {
        entity.ToTable(tableName);
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        entity.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class MetivonDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<MetivonDbContext>
{
    public MetivonDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        var options = new DbContextOptionsBuilder<MetivonDbContext>().UseSqlServer(configuration.GetConnectionString("DefaultConnection")).Options;
        return new MetivonDbContext(options);
    }
}
