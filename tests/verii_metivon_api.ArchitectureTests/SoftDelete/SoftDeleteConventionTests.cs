using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Persistence;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.SoftDelete;

public sealed class SoftDeleteConventionTests
{
    private static MetivonDbContext CreateModelContext()
    {
        var options = new DbContextOptionsBuilder<MetivonDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MetivonSoftDeleteModelContract;Trusted_Connection=True")
            .Options;
        return new MetivonDbContext(options);
    }

    [Fact]
    public void Every_entity_is_hidden_by_a_query_filter_after_soft_delete()
    {
        using var db = CreateModelContext();
        var entities = db.Model.GetEntityTypes()
            .Where(x => typeof(Entity).IsAssignableFrom(x.ClrType))
            .ToArray();

        Assert.NotEmpty(entities);
        Assert.All(entities, entity =>
            Assert.Contains(nameof(Entity.IsDeleted), entity.GetQueryFilter()?.ToString(), StringComparison.Ordinal));
    }

    [Fact]
    public void Every_unique_business_index_ignores_soft_deleted_rows()
    {
        using var db = CreateModelContext();
        var uniqueIndexes = db.Model.GetEntityTypes()
            .Where(x => typeof(Entity).IsAssignableFrom(x.ClrType))
            .SelectMany(x => x.GetIndexes())
            .Where(x => x.IsUnique)
            .ToArray();

        Assert.NotEmpty(uniqueIndexes);
        Assert.All(uniqueIndexes, index =>
            Assert.Contains(nameof(Entity.IsDeleted), index.GetFilter(), StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DbContext_owns_the_save_pipeline_that_converts_physical_deletes()
    {
        var method = typeof(MetivonDbContext).GetMethod(
            nameof(DbContext.SaveChangesAsync),
            [typeof(bool), typeof(CancellationToken)]);

        Assert.NotNull(method);
        Assert.Equal(typeof(MetivonDbContext), method!.DeclaringType);
    }
}
