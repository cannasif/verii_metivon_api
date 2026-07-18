using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Modules.NumberSeries.Domain.Entities;

namespace verii_metivon_api.Modules.NumberSeries.Infrastructure.Persistence.Configurations;

public sealed class DocumentNumberSeriesConfiguration : IEntityTypeConfiguration<DocumentNumberSeries>
{
    public void Configure(EntityTypeBuilder<DocumentNumberSeries> b)
    {
        b.ToTable("RII_DOCUMENT_NUMBER_SERIES",t=>t.HasCheckConstraint("CK_RII_DOCUMENT_NUMBER_SERIES_RESERVATION_TIMEOUT","[ReservationTimeoutMinutes] >= 1 AND [ReservationTimeoutMinutes] <= 1440"));
        b.HasQueryFilter(x => !x.IsDeleted);
        b.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasIndex(x => new { x.Module, x.Reference, x.BranchId, x.WarehouseId, x.Priority });
        b.Property(x => x.Code).HasMaxLength(20).IsRequired();
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Module).HasMaxLength(80).IsRequired();
        b.Property(x => x.Reference).HasMaxLength(120).IsRequired();
        b.Property(x => x.Format).HasMaxLength(160).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}

public sealed class NumberSeriesAssignmentConfiguration : IEntityTypeConfiguration<NumberSeriesAssignment>
{
    public void Configure(EntityTypeBuilder<NumberSeriesAssignment> b)
    {
        b.ToTable("RII_NUMBER_SERIES_ASSIGNMENTS");
        b.HasQueryFilter(x => !x.IsDeleted);
        b.HasIndex(x => new { x.NumberSeriesId, x.BranchId, x.WarehouseId, x.UserId, x.BusinessPartnerId, x.Channel, x.Scenario }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.Channel).HasMaxLength(40);
        b.Property(x => x.Scenario).HasMaxLength(40);
        b.HasOne(x => x.NumberSeries).WithMany(x => x.Assignments).HasForeignKey(x => x.NumberSeriesId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class NumberSeriesCounterConfiguration : IEntityTypeConfiguration<NumberSeriesCounter>
{
    public void Configure(EntityTypeBuilder<NumberSeriesCounter> b)
    {
        b.ToTable("RII_NUMBER_SERIES_COUNTERS");
        b.HasQueryFilter(x => !x.IsDeleted);
        b.HasIndex(x => new { x.NumberSeriesId, x.PeriodKey }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.PeriodKey).HasMaxLength(10).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasOne(x => x.NumberSeries).WithMany(x => x.Counters).HasForeignKey(x => x.NumberSeriesId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class NumberSeriesUsageConfiguration : IEntityTypeConfiguration<NumberSeriesUsage>
{
    public void Configure(EntityTypeBuilder<NumberSeriesUsage> b)
    {
        b.ToTable("RII_NUMBER_SERIES_USAGES");
        b.HasQueryFilter(x => !x.IsDeleted);
        b.HasIndex(x => new { x.NumberSeriesId, x.DocumentNumber }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasIndex(x => new { x.DocumentType, x.DocumentId });
        b.HasIndex(x => new { x.Status, x.ReservationExpiresAt });
        b.Property(x => x.DocumentNumber).HasMaxLength(160).IsRequired();
        b.Property(x => x.DocumentType).HasMaxLength(120).IsRequired();
        b.Property(x => x.PeriodKey).HasMaxLength(10).IsRequired();
        b.Property(x => x.CancellationReason).HasMaxLength(500);
        b.HasOne(x => x.NumberSeries).WithMany().HasForeignKey(x => x.NumberSeriesId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.NumberSeriesCounter).WithMany().HasForeignKey(x => x.NumberSeriesCounterId).OnDelete(DeleteBehavior.Restrict);
    }
}
