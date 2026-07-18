using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.Organization.Infrastructure.Persistence.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> entity)
    {
        entity.ToTable("RII_BRANCHES");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Code).HasMaxLength(30).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
        entity.HasIndex(x => x.Code).IsUnique();
        entity.HasQueryFilter(x => !x.IsDeleted);
    }
}
