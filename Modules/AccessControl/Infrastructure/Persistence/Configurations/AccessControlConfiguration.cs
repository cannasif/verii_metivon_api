using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Modules.AccessControl.Infrastructure.Persistence.Configurations;

public sealed class PermissionDefinitionConfiguration : IEntityTypeConfiguration<PermissionDefinition>
{
    public void Configure(EntityTypeBuilder<PermissionDefinition> e) { e.ToTable("RII_PERMISSION_DEFINITIONS"); e.HasKey(x => x.Id); e.Property(x => x.Code).HasMaxLength(180).IsRequired(); e.Property(x => x.Name).HasMaxLength(180).IsRequired(); e.Property(x => x.Description).HasMaxLength(500); e.Property(x => x.Module).HasMaxLength(80).IsRequired(); e.Property(x => x.Resource).HasMaxLength(100).IsRequired(); e.Property(x => x.Action).HasMaxLength(40).IsRequired(); e.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasQueryFilter(x => !x.IsDeleted); }
}
public sealed class PermissionGroupConfiguration : IEntityTypeConfiguration<PermissionGroup>
{
    public void Configure(EntityTypeBuilder<PermissionGroup> e) { e.ToTable("RII_PERMISSION_GROUPS"); e.HasKey(x => x.Id); e.Property(x => x.Code).HasMaxLength(80).IsRequired(); e.Property(x => x.Name).HasMaxLength(120).IsRequired(); e.Property(x => x.Description).HasMaxLength(500); e.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasQueryFilter(x => !x.IsDeleted); }
}
public sealed class PermissionGroupPermissionConfiguration : IEntityTypeConfiguration<PermissionGroupPermission>
{
    public void Configure(EntityTypeBuilder<PermissionGroupPermission> e) { e.ToTable("RII_PERMISSION_GROUP_PERMISSIONS"); e.HasKey(x => x.Id); e.HasIndex(x => new { x.PermissionGroupId, x.PermissionDefinitionId }).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasOne(x => x.PermissionGroup).WithMany(x => x.GroupPermissions).HasForeignKey(x => x.PermissionGroupId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.PermissionDefinition).WithMany(x => x.GroupPermissions).HasForeignKey(x => x.PermissionDefinitionId).OnDelete(DeleteBehavior.Cascade); e.HasQueryFilter(x => !x.IsDeleted); }
}
public sealed class UserPermissionGroupConfiguration : IEntityTypeConfiguration<UserPermissionGroup>
{
    public void Configure(EntityTypeBuilder<UserPermissionGroup> e) { e.ToTable("RII_USER_PERMISSION_GROUPS"); e.HasKey(x => x.Id); e.HasIndex(x => new { x.UserId, x.PermissionGroupId }).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.PermissionGroup).WithMany(x => x.UserGroups).HasForeignKey(x => x.PermissionGroupId).OnDelete(DeleteBehavior.Cascade); e.HasQueryFilter(x => !x.IsDeleted); }
}
public sealed class VisibilityPolicyConfiguration : IEntityTypeConfiguration<VisibilityPolicy>
{
    public void Configure(EntityTypeBuilder<VisibilityPolicy> e) { e.ToTable("RII_VISIBILITY_POLICIES"); e.HasKey(x => x.Id); e.Property(x => x.Code).HasMaxLength(120).IsRequired(); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.Property(x => x.EntityType).HasMaxLength(60).IsRequired(); e.Property(x => x.Description).HasMaxLength(500); e.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasIndex(x => new { x.EntityType, x.Priority }); e.HasQueryFilter(x => !x.IsDeleted); }
}
public sealed class UserVisibilityPolicyConfiguration : IEntityTypeConfiguration<UserVisibilityPolicy>
{
    public void Configure(EntityTypeBuilder<UserVisibilityPolicy> e) { e.ToTable("RII_USER_VISIBILITY_POLICIES"); e.HasKey(x => x.Id); e.HasIndex(x => new { x.UserId, x.VisibilityPolicyId }).IsUnique().HasFilter("[IsDeleted] = 0"); e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.VisibilityPolicy).WithMany(x => x.UserAssignments).HasForeignKey(x => x.VisibilityPolicyId).OnDelete(DeleteBehavior.Cascade); e.HasQueryFilter(x => !x.IsDeleted); }
}
public sealed class AccessControlAuditLogConfiguration : IEntityTypeConfiguration<AccessControlAuditLog>
{
    public void Configure(EntityTypeBuilder<AccessControlAuditLog> e) { e.ToTable("RII_ACCESS_CONTROL_AUDIT_LOGS"); e.HasKey(x => x.Id); e.Property(x => x.TraceId).HasMaxLength(64).IsRequired(); e.Property(x => x.ActionType).HasMaxLength(80).IsRequired(); e.Property(x => x.EntityType).HasMaxLength(100); e.Property(x => x.EntityId).HasMaxLength(100); e.Property(x => x.Result).HasMaxLength(30).IsRequired(); e.Property(x => x.Source).HasMaxLength(80).IsRequired(); e.Property(x => x.BranchCode).HasMaxLength(50); e.Property(x => x.RequestPath).HasMaxLength(500); e.Property(x => x.RequestMethod).HasMaxLength(20); e.Property(x => x.PerformedByUserEmail).HasMaxLength(200); e.HasIndex(x => x.TraceId); e.HasIndex(x => new { x.EntityType, x.EntityId }); e.HasIndex(x => x.CreatedAt); e.HasQueryFilter(x => !x.IsDeleted); }
}
