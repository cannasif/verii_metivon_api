using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Inventory.Application.Services;
using verii_metivon_api.Modules.Inventory.Domain.Entities;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Warehouses.Domain.Entities;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.Inventory;

public sealed class InventoryPostingModelTests
{
    private static MetivonDbContext CreateModelContext()
    {
        var options = new DbContextOptionsBuilder<MetivonDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MetivonInventoryModelContract;Trusted_Connection=True")
            .Options;
        return new MetivonDbContext(options);
    }

    [Fact]
    public void Inventory_balance_dimensions_are_referentially_protected()
    {
        using var db = CreateModelContext();
        var balance = db.Model.FindEntityType(typeof(InventoryBalance))!;
        var principalTypes = balance.GetForeignKeys().Select(x => x.PrincipalEntityType.ClrType).ToHashSet();

        Assert.Contains(typeof(Product), principalTypes);
        Assert.Contains(typeof(Warehouse), principalTypes);
        Assert.Contains(typeof(StorageLocation), principalTypes);
        Assert.Contains(typeof(InventoryStatus), principalTypes);
        Assert.Contains(typeof(InventoryLot), principalTypes);
        Assert.Contains(typeof(InventorySerial), principalTypes);
    }

    [Fact]
    public void Inventory_ledger_supports_posting_and_document_trace_queries()
    {
        using var db = CreateModelContext();
        var transaction = db.Model.FindEntityType(typeof(InventoryTransaction))!;

        Assert.Contains(transaction.GetIndexes(), index =>
            index.Properties.Select(property => property.Name).SequenceEqual([nameof(InventoryTransaction.PostingId)]));
        Assert.Contains(transaction.GetIndexes(), index =>
            index.Properties.Select(property => property.Name).SequenceEqual([
                nameof(InventoryTransaction.DocumentType), nameof(InventoryTransaction.DocumentId)]));
    }

    [Fact]
    public void Posting_result_distinguishes_an_idempotent_replay()
    {
        var firstPosting = new PostInventoryResult(Guid.NewGuid(), [1]);
        var replay = new PostInventoryResult(firstPosting.PostingId, [1], true);

        Assert.False(firstPosting.AlreadyPosted);
        Assert.True(replay.AlreadyPosted);
    }
}
