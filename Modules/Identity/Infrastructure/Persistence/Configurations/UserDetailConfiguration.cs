using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
{
    public void Configure(EntityTypeBuilder<UserDetail> entity)
    {
        entity.ToTable("RII_USER_DETAILS");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.FirstName).HasMaxLength(100);
        entity.Property(x => x.LastName).HasMaxLength(100);
        entity.Property(x => x.PhoneNumber).HasMaxLength(30);
        entity.Property(x => x.ProfilePictureUrl).HasMaxLength(500);
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.HasIndex(x => x.UserId).IsUnique();
        entity.HasOne(x => x.User).WithOne(x => x.Detail).HasForeignKey<UserDetail>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        entity.HasQueryFilter(x => !x.IsDeleted);
    }
}
