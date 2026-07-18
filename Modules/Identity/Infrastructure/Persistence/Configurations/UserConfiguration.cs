using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("RII_USERS");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
        entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        entity.Property(x => x.Role).HasMaxLength(50).IsRequired();
        entity.Property(x => x.RefreshToken).HasMaxLength(200);
        entity.HasIndex(x => x.Username).IsUnique();
        entity.HasIndex(x => x.Email).IsUnique();
        entity.HasOne(x => x.Branch).WithMany(x => x.Users).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        entity.HasQueryFilter(x => !x.IsDeleted);
    }
}
