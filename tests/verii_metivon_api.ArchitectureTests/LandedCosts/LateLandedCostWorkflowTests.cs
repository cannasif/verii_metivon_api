using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.LandedCosts.Application.Services;
using verii_metivon_api.Modules.LandedCosts.Domain.Entities;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.LandedCosts;

public sealed class LateLandedCostWorkflowTests
{
    [Fact]
    public async Task Adjustment_cost_reopens_a_finalized_profile_for_reallocation()
    {
        await using var db = CreateContext();
        var currency = new Currency { Code = "TRY", Name = "Turkish Lira", IsoCode = "TRY", Symbol = "₺", IsDefault = true, IsActive = true };
        var costType = new LandedCostType { Code = "LATE", Name = "Late cost", DefaultAllocationMethod = LandedCostAllocationMethod.Amount, IsActive = true };
        db.AddRange(currency, costType);
        await db.SaveChangesAsync();

        var dossier = new ImportDossier
        {
            DossierNumber = "TEST-LATE-COST",
            Status = ImportDossierStatus.Finalized,
            BranchId = 1,
            SupplierId = 1,
            CurrencyId = currency.Id,
            CurrencyCode = currency.IsoCode,
            IncotermCode = "FOB",
            OpenDate = DateOnly.FromDateTime(DateTime.UtcNow),
            FinalizedAt = DateTime.UtcNow,
        };
        db.ImportDossiers.Add(dossier);
        await db.SaveChangesAsync();

        var service = new LandedCostService(db);
        var result = await service.AddCostAsync(dossier.Id, CreateRequest(costType.Id, currency.Id, LandedCostAmountType.Adjustment), CancellationToken.None);

        Assert.True(result.Success, result.Message);
        var saved = await db.ImportDossiers.Include(x => x.Costs).SingleAsync(x => x.Id == dossier.Id);
        Assert.Equal(ImportDossierStatus.ReadyForAllocation, saved.Status);
        Assert.Null(saved.FinalizedAt);
        Assert.Single(saved.Costs);
        Assert.Equal(125m, saved.Costs.Single().LocalAmount);
    }

    [Fact]
    public async Task Actual_cost_cannot_silently_change_a_finalized_profile()
    {
        await using var db = CreateContext();
        var currency = new Currency { Code = "TRY", Name = "Turkish Lira", IsoCode = "TRY", Symbol = "₺", IsDefault = true, IsActive = true };
        var costType = new LandedCostType { Code = "ACTUAL", Name = "Actual cost", DefaultAllocationMethod = LandedCostAllocationMethod.Amount, IsActive = true };
        db.AddRange(currency, costType);
        await db.SaveChangesAsync();
        var dossier = new ImportDossier { DossierNumber = "TEST-FINAL", Status = ImportDossierStatus.Finalized, BranchId = 1, SupplierId = 1, CurrencyId = currency.Id, CurrencyCode = currency.IsoCode, IncotermCode = "FOB", OpenDate = DateOnly.FromDateTime(DateTime.UtcNow), FinalizedAt = DateTime.UtcNow };
        db.ImportDossiers.Add(dossier);
        await db.SaveChangesAsync();

        var result = await new LandedCostService(db).AddCostAsync(dossier.Id, CreateRequest(costType.Id, currency.Id, LandedCostAmountType.Actual), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Empty(db.ImportDossierCosts);
    }

    [Fact]
    public async Task Actual_cost_still_saves_on_an_open_profile()
    {
        await using var db = CreateContext();
        var currency = new Currency { Code = "TRY", Name = "Turkish Lira", IsoCode = "TRY", Symbol = "₺", IsDefault = true, IsActive = true };
        var costType = new LandedCostType { Code = "OPEN", Name = "Open profile cost", DefaultAllocationMethod = LandedCostAllocationMethod.Amount, IsActive = true };
        db.AddRange(currency, costType);
        await db.SaveChangesAsync();
        var dossier = new ImportDossier { DossierNumber = "TEST-OPEN", Status = ImportDossierStatus.ReceivedProvisionally, BranchId = 1, SupplierId = 1, CurrencyId = currency.Id, CurrencyCode = currency.IsoCode, IncotermCode = "FOB", OpenDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        db.ImportDossiers.Add(dossier);
        await db.SaveChangesAsync();

        var result = await new LandedCostService(db).AddCostAsync(dossier.Id, CreateRequest(costType.Id, currency.Id, LandedCostAmountType.Actual), CancellationToken.None);

        Assert.True(result.Success, result.Message);
        Assert.Equal(ImportDossierStatus.ReadyForAllocation, dossier.Status);
        Assert.Single(db.ImportDossierCosts);
    }

    [Fact]
    public async Task Closed_profile_rejects_even_an_adjustment_cost()
    {
        await using var db = CreateContext();
        var currency = new Currency { Code = "TRY", Name = "Turkish Lira", IsoCode = "TRY", Symbol = "₺", IsDefault = true, IsActive = true };
        var costType = new LandedCostType { Code = "CLOSED", Name = "Closed profile cost", DefaultAllocationMethod = LandedCostAllocationMethod.Amount, IsActive = true };
        db.AddRange(currency, costType);
        await db.SaveChangesAsync();
        var dossier = new ImportDossier { DossierNumber = "TEST-CLOSED", Status = ImportDossierStatus.Closed, BranchId = 1, SupplierId = 1, CurrencyId = currency.Id, CurrencyCode = currency.IsoCode, IncotermCode = "FOB", OpenDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        db.ImportDossiers.Add(dossier);
        await db.SaveChangesAsync();

        var result = await new LandedCostService(db).AddCostAsync(dossier.Id, CreateRequest(costType.Id, currency.Id, LandedCostAmountType.Adjustment), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Empty(db.ImportDossierCosts);
    }

    private static AddDossierCostRequest CreateRequest(long costTypeId, long currencyId, LandedCostAmountType amountType) =>
        new(costTypeId, amountType, null, LandedCostSourceType.PurchaseInvoice, null, "TEST-INVOICE", null, null, null, currencyId, 125m, 1m, 1m, DateOnly.FromDateTime(DateTime.UtcNow), "BaseCurrency", "Automated late-cost test", null);

    private static MetivonDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MetivonDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new MetivonDbContext(options);
    }
}
