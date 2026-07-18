using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Warehouses.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Entities;
using verii_metivon_api.Modules.Accounting.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Enums;
using verii_metivon_api.Modules.LandedCosts.Domain.Entities;
using verii_metivon_api.Modules.Parameters.Domain.Entities;
using verii_metivon_api.Modules.NumberSeries.Domain.Entities;
using verii_metivon_api.Modules.AccessControl.Infrastructure.Persistence;

namespace verii_metivon_api.Core.Persistence;

public static class SeedData
{
    public static async Task ApplyAsync(MetivonDbContext db, IConfiguration configuration)
    {
        var branch = await db.Branches.FirstOrDefaultAsync(x => x.Code == "0");
        if (branch is null)
        {
            branch = new Branch { Code = "0", Name = "Default Branch", IsDefault = true, IsActive = true };
            db.Branches.Add(branch);
            await db.SaveChangesAsync();
        }

        if (!await db.BusinessPartnerTypes.AnyAsync())
            db.BusinessPartnerTypes.AddRange(
                new BusinessPartnerType { Code = "CUSTOMER", Name = "Customer", IsDefault = true, DisplayOrder = 10 },
                new BusinessPartnerType { Code = "SUPPLIER", Name = "Supplier", DisplayOrder = 20 },
                new BusinessPartnerType { Code = "LEAD", Name = "Lead", DisplayOrder = 30 });
        if (!await db.CustomerGroups.AnyAsync())
            db.CustomerGroups.Add(new CustomerGroup { Code = "DEFAULT", Name = "Default Customer Group", IsDefault = true });
        if (!await db.PaymentTerms.AnyAsync())
            db.PaymentTerms.AddRange(
                new PaymentTerm { Code = "CASH", Name = "Cash", DueDays = 0, IsDefault = true },
                new PaymentTerm { Code = "NET30", Name = "Net 30 Days", DueDays = 30 },
                new PaymentTerm { Code = "NET60", Name = "Net 60 Days", DueDays = 60 });
        if (!await db.Currencies.AnyAsync())
            db.Currencies.AddRange(
                new Currency { Code = "TRY", Name = "Turkish Lira", IsoCode = "TRY", Symbol = "₺", IsDefault = true },
                new Currency { Code = "USD", Name = "US Dollar", IsoCode = "USD", Symbol = "$" },
                new Currency { Code = "EUR", Name = "Euro", IsoCode = "EUR", Symbol = "€" });
        if (!await db.TaxGroups.AnyAsync())
            db.TaxGroups.AddRange(
                new TaxGroup { Code = "DOMESTIC", Name = "Domestic", IsDefault = true },
                new TaxGroup { Code = "EXPORT", Name = "Export" },
                new TaxGroup { Code = "EXEMPT", Name = "Tax Exempt", IsTaxExempt = true });
        await db.SaveChangesAsync();

        await SeedProductDefinitionsAsync(db);
        await SeedWarehouseDefinitionsAsync(db);
        await SeedDefaultWarehouseAsync(db, branch.Id);
        await SeedInventoryDefinitionsAsync(db);
        await SeedAccountingDefinitionsAsync(db);
        await SeedLandedCostDefinitionsAsync(db);
        await SeedParametersAsync(db);
        await SeedNumberSeriesAsync(db, branch.Id);

        var email = configuration["BootstrapAdmin:Email"];
        var password = configuration["BootstrapAdmin:Password"];
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && !await db.Users.AnyAsync())
        {
            var user = new User
            {
                Username = email,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
                BranchId = branch.Id,
                IsActive = true,
                Detail = new UserDetail { FirstName = "V3RII", LastName = "Admin" }
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        await DefaultPermissionGroupSeeder.ApplyAsync(db, email);
    }

    private static async Task SeedDefaultWarehouseAsync(MetivonDbContext db,long branchId)
    {
        if (await db.Warehouses.AnyAsync()) return;
        var warehouseType=await db.WarehouseTypes.FirstAsync(x=>x.Code=="STANDARD");
        var warehouse=new Warehouse { Code="MAIN", Name="Main Warehouse", BranchId=branchId, WarehouseTypeId=warehouseType.Id, IsDefault=true, IsWmsEnabled=true, IsActive=true, CountryCode="TR" };
        db.Warehouses.Add(warehouse);await db.SaveChangesAsync();
        var storageType=await db.LocationTypes.FirstAsync(x=>x.Code=="STORAGE");var receivingType=await db.LocationTypes.FirstAsync(x=>x.Code=="RECEIVING");var shippingType=await db.LocationTypes.FirstAsync(x=>x.Code=="SHIPPING");var quarantineType=await db.LocationTypes.FirstAsync(x=>x.Code=="QUARANTINE");
        var zone=new WarehouseZone { WarehouseId=warehouse.Id,Code="MAIN",Name="Main Storage",ZonePurpose=0,PickPriority=10,PutawayPriority=10 };
        db.WarehouseZones.Add(zone);await db.SaveChangesAsync();
        db.StorageLocations.AddRange(new StorageLocation{WarehouseId=warehouse.Id,WarehouseZoneId=zone.Id,LocationTypeId=storageType.Id,Code="A-01-01-01",Aisle="A",Bay="01",Level="01",Position="01"},new StorageLocation{WarehouseId=warehouse.Id,LocationTypeId=receivingType.Id,Code="RECEIVING",IsReceiving=true},new StorageLocation{WarehouseId=warehouse.Id,LocationTypeId=shippingType.Id,Code="SHIPPING",IsShipping=true},new StorageLocation{WarehouseId=warehouse.Id,LocationTypeId=quarantineType.Id,Code="QUARANTINE",IsQuarantine=true});
        await db.SaveChangesAsync();
    }

    private static async Task SeedParametersAsync(MetivonDbContext db)
    {
        if (!await db.NumberSequences.AnyAsync(x => x.Module == "BusinessPartners" && x.Reference == "BusinessPartnerCode"))
            db.NumberSequences.Add(new NumberSequence { Module="BusinessPartners", Reference="BusinessPartnerCode", Format="CAR-{NUMBER:6}", CurrentNumber=1, IncrementBy=1, MinimumNumber=1, MaximumNumber=999999, IsAutomatic=true, AllowManual=true, IsContinuous=false, IsActive=true });
        if (!await db.ErpParameters.AnyAsync(x => x.Module == "BusinessPartners"))
            db.ErpParameters.AddRange(
                new ErpParameter { Module="BusinessPartners", Key="ForceUppercase", ValueType="Boolean", Value="True", DisplayOrder=10, Description="Normalize business partner codes to uppercase." },
                new ErpParameter { Module="BusinessPartners", Key="TrimWhitespace", ValueType="Boolean", Value="True", DisplayOrder=20, Description="Trim leading and trailing whitespace from business partner codes." });
        await db.SaveChangesAsync();
    }

    private static async Task SeedNumberSeriesAsync(MetivonDbContext db,long branchId)
    {
        if(await db.DocumentNumberSeries.AnyAsync())return;
        db.DocumentNumberSeries.AddRange(
            Series("GR","Goods Receipt","Receiving","GoodsReceiptNumber","GR-{YYYY}-{NUMBER:6}",branchId),
            Series("TR","Warehouse Transfer","Transfers","TransferNumber","TR-{YYYY}-{NUMBER:6}",branchId),
            Series("SH","Shipment","Shipping","ShipmentNumber","SH-{YYYY}-{NUMBER:6}",branchId),
            Series("DN","Delivery Note","Shipping","DeliveryNoteNumber","DN-{YYYY}-{NUMBER:6}",branchId),
            Series("PO","Purchase Order","Procurement","PurchaseOrderNumber","PO-{YYYY}-{NUMBER:6}",branchId),
            Series("SO","Sales Order","Sales","SalesOrderNumber","SO-{YYYY}-{NUMBER:6}",branchId),
            Series("EF1","E-Invoice","EDocuments","EInvoiceNumber","{SERIES}{YYYY}{NUMBER:9}",branchId,true),
            Series("EA1","E-Archive Invoice","EDocuments","EArchiveNumber","{SERIES}{YYYY}{NUMBER:9}",branchId,true),
            Series("EI1","E-Despatch","EDocuments","EDespatchNumber","{SERIES}{YYYY}{NUMBER:9}",branchId,true));
        await db.SaveChangesAsync();
        static DocumentNumberSeries Series(string code,string name,string module,string reference,string format,long branch,bool gib=false)=>new(){Code=code,Name=name,Module=module,Reference=reference,ScopeType=NumberSeriesScopeType.Branch,BranchId=branch,Format=format,ResetPeriod=NumberSeriesResetPeriod.Yearly,StartingNumber=1,IncrementBy=1,MaximumNumber=gib?999999999:999999,IsGibCompliant=gib,AllowManual=!gib,IsContinuous=gib,IsDefault=true,Priority=100,IsActive=true};
    }

    private static async Task SeedAccountingDefinitionsAsync(MetivonDbContext db)
    {
        if (!await db.LedgerAccounts.AnyAsync())
            db.LedgerAccounts.AddRange(
                A("120","Trade Receivables",AccountType.Asset), A("153","Inventory",AccountType.Asset), A("191","Input VAT",AccountType.Asset),
                A("320","Trade Payables",AccountType.Liability), A("391","Output VAT",AccountType.Liability), A("600","Domestic Sales",AccountType.Revenue),
                A("621","Cost of Goods Sold",AccountType.Expense), A("689","Inventory Losses",AccountType.Expense), A("679","Inventory Gains",AccountType.Revenue),
                A("159","Goods Received Not Invoiced",AccountType.Liability));
        if (!await db.FiscalPeriods.AnyAsync())
            db.FiscalPeriods.Add(new FiscalPeriod { Code="2026", Name="Fiscal Year 2026", StartDate=new DateOnly(2026,1,1), EndDate=new DateOnly(2026,12,31), IsOpen=true });
        await db.SaveChangesAsync();
        if (!await db.InventoryPostingProfiles.AnyAsync())
        {
            var accounts=await db.LedgerAccounts.ToDictionaryAsync(x=>x.Code);
            db.InventoryPostingProfiles.AddRange(
                P("PURCHASE_RECEIPT",InventoryMovementType.PurchaseReceipt,accounts["153"].Id,accounts["159"].Id),
                P("SALES_SHIPMENT",InventoryMovementType.SalesShipment,accounts["621"].Id,accounts["153"].Id),
                P("COUNT_GAIN",InventoryMovementType.CountGain,accounts["153"].Id,accounts["679"].Id),
                P("COUNT_LOSS",InventoryMovementType.CountLoss,accounts["689"].Id,accounts["153"].Id),
                P("ADJUST_IN",InventoryMovementType.AdjustmentIn,accounts["153"].Id,accounts["679"].Id),
                P("ADJUST_OUT",InventoryMovementType.AdjustmentOut,accounts["689"].Id,accounts["153"].Id));
            await db.SaveChangesAsync();
        }
    }

    private static LedgerAccount A(string code,string name,AccountType type)=>new(){Code=code,Name=name,AccountType=type};
    private static InventoryPostingProfile P(string code,InventoryMovementType type,long debit,long credit)=>new(){Code=code,Name=code.Replace('_',' '),MovementType=type,DebitAccountId=debit,CreditAccountId=credit,Priority=100};

    private static async Task SeedLandedCostDefinitionsAsync(MetivonDbContext db)
    {
        if (await db.LandedCostTypes.AnyAsync()) return;
        var accounts = await db.LedgerAccounts.ToDictionaryAsync(x => x.Code);
        var clearing = accounts["159"].Id;
        var variance = accounts["689"].Id;
        db.LandedCostTypes.AddRange(
            L("FREIGHT", "Freight", LandedCostAllocationMethod.Weight, clearing, variance, true),
            L("INSURANCE", "Insurance", LandedCostAllocationMethod.Amount, clearing, variance, true),
            L("CUSTOMS_DUTY", "Customs Duty", LandedCostAllocationMethod.Amount, clearing, variance, true, true),
            L("STORAGE", "Customs Storage", LandedCostAllocationMethod.Volume, clearing, variance, true),
            L("BROKERAGE", "Customs Brokerage", LandedCostAllocationMethod.Amount, clearing, variance, true),
            L("HANDLING", "Handling", LandedCostAllocationMethod.Quantity, clearing, variance, true),
            L("BANK_COMMISSION", "Bank Commission", LandedCostAllocationMethod.Amount, clearing, variance, true),
            L("CERTIFICATION", "Certification and Inspection", LandedCostAllocationMethod.Manual, clearing, variance, true));
        await db.SaveChangesAsync();
    }
    private static LandedCostType L(string code,string name,LandedCostAllocationMethod method,long clearing,long variance,bool capitalize,bool customs=false)=>new(){Code=code,Name=name,DefaultAllocationMethod=method,ClearingAccountId=clearing,VarianceAccountId=variance,CapitalizeToInventory=capitalize,IncludeInCustomsValue=customs,IsActive=true,DisplayOrder=10};

    private static async Task SeedInventoryDefinitionsAsync(MetivonDbContext db)
    {
        if (await db.InventoryStatuses.AnyAsync()) return;
        db.InventoryStatuses.AddRange(
            new InventoryStatus { Code="AVAILABLE", Name="Available", IsDefault=true, DisplayOrder=10 },
            new InventoryStatus { Code="QUALITY", Name="Quality Inspection", IsAvailable=false, IsReservable=false, DisplayOrder=20 },
            new InventoryStatus { Code="QUARANTINE", Name="Quarantine", IsAvailable=false, IsReservable=false, IsNettable=false, DisplayOrder=30 },
            new InventoryStatus { Code="BLOCKED", Name="Blocked", IsAvailable=false, IsReservable=false, IsNettable=false, DisplayOrder=40 },
            new InventoryStatus { Code="DAMAGED", Name="Damaged", IsAvailable=false, IsReservable=false, IsNettable=false, DisplayOrder=50 },
            new InventoryStatus { Code="CUSTOMS_HOLD", Name="Customs Hold", IsAvailable=false, IsReservable=false, IsNettable=false, DisplayOrder=60 },
            new InventoryStatus { Code="BONDED", Name="Bonded Warehouse", IsAvailable=false, IsReservable=false, IsNettable=false, DisplayOrder=70 },
            new InventoryStatus { Code="EXPORT_HOLD", Name="Export Customs Hold", IsAvailable=false, IsReservable=false, IsNettable=false, DisplayOrder=80 });
        await db.SaveChangesAsync();
    }

    private static async Task SeedWarehouseDefinitionsAsync(MetivonDbContext db)
    {
        if (!await db.WarehouseTypes.AnyAsync())
            db.WarehouseTypes.AddRange(
                new WarehouseType { Code = "STANDARD", Name = "Standard Warehouse", IsDefault = true, DisplayOrder = 10 },
                new WarehouseType { Code = "DISTRIBUTION", Name = "Distribution Center", DisplayOrder = 20 },
                new WarehouseType { Code = "TRANSIT", Name = "Transit Warehouse", DisplayOrder = 30 },
                new WarehouseType { Code = "CONSIGNMENT", Name = "Consignment Warehouse", DisplayOrder = 40 });
        if (!await db.LocationTypes.AnyAsync())
            db.LocationTypes.AddRange(
                new LocationType { Code = "STORAGE", Name = "Storage", IsDefault = true, DisplayOrder = 10 },
                new LocationType { Code = "RECEIVING", Name = "Receiving", IsPickable = false, DisplayOrder = 20 },
                new LocationType { Code = "SHIPPING", Name = "Shipping", IsReceivable = false, DisplayOrder = 30 },
                new LocationType { Code = "PICKING", Name = "Picking", DisplayOrder = 40 },
                new LocationType { Code = "QUARANTINE", Name = "Quarantine", IsPickable = false, DisplayOrder = 50 },
                new LocationType { Code = "DAMAGED", Name = "Damaged Stock", IsPickable = false, DisplayOrder = 60 });
        await db.SaveChangesAsync();
    }

    private static async Task SeedProductDefinitionsAsync(MetivonDbContext db)
    {
        if (!await db.UnitCategories.AnyAsync())
            db.UnitCategories.AddRange(
                new UnitCategory { Code = "QUANTITY", Name = "Quantity", IsDefault = true, DisplayOrder = 10 },
                new UnitCategory { Code = "WEIGHT", Name = "Weight", DisplayOrder = 20 },
                new UnitCategory { Code = "LENGTH", Name = "Length", DisplayOrder = 30 },
                new UnitCategory { Code = "AREA", Name = "Area", DisplayOrder = 40 },
                new UnitCategory { Code = "VOLUME", Name = "Volume", DisplayOrder = 50 },
                new UnitCategory { Code = "TIME", Name = "Time", DisplayOrder = 60 });
        await db.SaveChangesAsync();

        if (!await db.Units.AnyAsync())
        {
            var categories = await db.UnitCategories.ToDictionaryAsync(x => x.Code);
            db.Units.AddRange(
                U("PCS", "Piece", "pcs", categories["QUANTITY"].Id, 1, true, 0), U("BOX", "Box", "box", categories["QUANTITY"].Id, 1, false, 0), U("PLT", "Pallet", "plt", categories["QUANTITY"].Id, 1, false, 0),
                U("KG", "Kilogram", "kg", categories["WEIGHT"].Id, 1, true, 3), U("GR", "Gram", "g", categories["WEIGHT"].Id, 0.001m, false, 3), U("TON", "Metric ton", "t", categories["WEIGHT"].Id, 1000, false, 3),
                U("M", "Meter", "m", categories["LENGTH"].Id, 1, true, 3), U("CM", "Centimeter", "cm", categories["LENGTH"].Id, 0.01m, false, 3), U("MM", "Millimeter", "mm", categories["LENGTH"].Id, 0.001m, false, 3),
                U("M2", "Square meter", "m²", categories["AREA"].Id, 1, true, 3), U("CM2", "Square centimeter", "cm²", categories["AREA"].Id, 0.0001m, false, 3),
                U("L", "Liter", "L", categories["VOLUME"].Id, 1, true, 3), U("ML", "Milliliter", "mL", categories["VOLUME"].Id, 0.001m, false, 3), U("M3", "Cubic meter", "m³", categories["VOLUME"].Id, 1000, false, 3),
                U("H", "Hour", "h", categories["TIME"].Id, 1, true, 2), U("MIN", "Minute", "min", categories["TIME"].Id, 1m / 60m, false, 2));
        }
        if (!await db.ProductCategories.AnyAsync()) db.ProductCategories.Add(new ProductCategory { Code = "GENERAL", Name = "General", IsDefault = true });
        if (!await db.ProductGroups.AnyAsync()) db.ProductGroups.AddRange(new ProductGroup { Code = "TRADE", Name = "Trading goods", IsDefault = true }, new ProductGroup { Code = "RAW", Name = "Raw materials" }, new ProductGroup { Code = "CONSUMABLE", Name = "Consumables" });
        if (!await db.Brands.AnyAsync()) db.Brands.Add(new Brand { Code = "GENERIC", Name = "Generic / Unbranded", IsDefault = true });
        if (!await db.PackageTypes.AnyAsync()) db.PackageTypes.AddRange(
            new PackageType { Code = "BOX", Name = "Box", IsDefault = true }, new PackageType { Code = "CARTON", Name = "Carton" }, new PackageType { Code = "PALLET", Name = "Pallet" },
            new PackageType { Code = "BAG", Name = "Bag" }, new PackageType { Code = "BOTTLE", Name = "Bottle" }, new PackageType { Code = "ROLL", Name = "Roll" }, new PackageType { Code = "CRATE", Name = "Crate" });
        await db.SaveChangesAsync();
    }

    private static Unit U(string code, string name, string symbol, long categoryId, decimal factor, bool isBase, int decimals) =>
        new() { Code = code, Name = name, Symbol = symbol, UnitCategoryId = categoryId, ConversionFactor = factor, IsBaseUnit = isBase, DecimalPlaces = decimals, IsDefault = isBase };
}
