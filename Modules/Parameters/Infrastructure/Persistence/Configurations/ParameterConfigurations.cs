using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Modules.Parameters.Domain.Entities;

namespace verii_metivon_api.Modules.Parameters.Infrastructure.Persistence.Configurations;

public sealed class ErpParameterConfiguration : IEntityTypeConfiguration<ErpParameter>
{
    public void Configure(EntityTypeBuilder<ErpParameter> b)
    {
        b.ToTable("RII_ERP_PARAMETERS");
        b.HasIndex(x => new { x.Module, x.Key, x.BranchId }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.Module).HasMaxLength(80).IsRequired(); b.Property(x => x.Key).HasMaxLength(120).IsRequired();
        b.Property(x => x.ValueType).HasMaxLength(30).IsRequired(); b.Property(x => x.Value).HasMaxLength(2000).IsRequired();
        b.Property(x => x.Description).HasMaxLength(500); b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
        b.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class NumberSequenceConfiguration : IEntityTypeConfiguration<NumberSequence>
{
    public void Configure(EntityTypeBuilder<NumberSequence> b)
    {
        b.ToTable("RII_NUMBER_SEQUENCES");
        b.HasIndex(x => new { x.Module, x.Reference, x.BranchId }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.Module).HasMaxLength(80).IsRequired(); b.Property(x => x.Reference).HasMaxLength(120).IsRequired();
        b.Property(x => x.Format).HasMaxLength(120).IsRequired(); b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
        b.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
