using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.LandedCosts.Domain.Entities;
using verii_metivon_api.Modules.TradeOperations.Api;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.LandedCosts;

public sealed class TradeDossierCostModelTests
{
    private static MetivonDbContext CreateModelContext()
    {
        var options = new DbContextOptionsBuilder<MetivonDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MetivonTradeCostModelContract;Trusted_Connection=True")
            .Options;
        return new MetivonDbContext(options);
    }

    [Fact]
    public void Trade_dossier_has_at_most_one_internal_cost_profile()
    {
        using var db = CreateModelContext();
        var dossier = db.Model.FindEntityType(typeof(ImportDossier))!;

        Assert.Contains(dossier.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(ImportDossier.TradeDossierId)]));
    }

    [Fact]
    public void Costs_are_owned_by_the_internal_profile_and_removed_with_it()
    {
        using var db = CreateModelContext();
        var cost = db.Model.FindEntityType(typeof(ImportDossierCost))!;
        var foreignKey = cost.GetForeignKeys().Single(key => key.PrincipalEntityType.ClrType == typeof(ImportDossier));

        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Theory]
    [InlineData(nameof(TradeDossiersController.Costs), "{id:long}/costs", "GET")]
    [InlineData(nameof(TradeDossiersController.AddCost), "{id:long}/costs", "POST")]
    [InlineData(nameof(TradeDossiersController.AllocateCosts), "{id:long}/costs/allocate", "POST")]
    [InlineData(nameof(TradeDossiersController.FinalizeCosts), "{id:long}/costs/finalize", "POST")]
    public void Cost_workflow_is_exposed_under_the_trade_dossier(string actionName, string template, string method)
    {
        var action = typeof(TradeDossiersController).GetMethod(actionName)!;
        var route = action.GetCustomAttributes(typeof(HttpMethodAttribute), false).Cast<HttpMethodAttribute>().Single();

        Assert.Equal(template, route.Template);
        Assert.Contains(method, route.HttpMethods);
    }
}
