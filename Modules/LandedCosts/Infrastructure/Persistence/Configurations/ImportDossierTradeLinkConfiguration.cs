using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Modules.LandedCosts.Domain.Entities;
using verii_metivon_api.Modules.TradeOperations.Domain.Entities;

namespace verii_metivon_api.Modules.LandedCosts.Infrastructure.Persistence.Configurations;

public sealed class ImportDossierTradeLinkConfiguration : IEntityTypeConfiguration<ImportDossier>
{
    public void Configure(EntityTypeBuilder<ImportDossier> builder)
    {
        builder.HasIndex(x => x.TradeDossierId).IsUnique().HasFilter("[TradeDossierId] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasOne<TradeDossier>().WithMany().HasForeignKey(x => x.TradeDossierId).OnDelete(DeleteBehavior.Restrict);
    }
}
