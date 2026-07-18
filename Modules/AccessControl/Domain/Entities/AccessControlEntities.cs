using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.AccessControl.Domain.Entities;

public sealed class PermissionDefinition : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool AvailableOnWeb { get; set; } = true;
    public bool AvailableOnMobile { get; set; }
    public bool IsSystem { get; set; }
    public ICollection<PermissionGroupPermission> GroupPermissions { get; set; } = [];
}

public sealed class PermissionGroup : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; }
    public int Priority { get; set; } = 100;
    public ICollection<PermissionGroupPermission> GroupPermissions { get; set; } = [];
    public ICollection<UserPermissionGroup> UserGroups { get; set; } = [];
}

public sealed class PermissionGroupPermission : Entity
{
    public long PermissionGroupId { get; set; }
    public PermissionGroup PermissionGroup { get; set; } = null!;
    public long PermissionDefinitionId { get; set; }
    public PermissionDefinition PermissionDefinition { get; set; } = null!;
    public bool IsDenied { get; set; }
}

public sealed class UserPermissionGroup : Entity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public long PermissionGroupId { get; set; }
    public PermissionGroup PermissionGroup { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

public enum VisibilityScopeType { Self = 1, ManagerHierarchy = 2, PermissionGroup = 3, Company = 4, Branch = 5, Warehouse = 6 }

public sealed class VisibilityPolicy : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public VisibilityScopeType ScopeType { get; set; } = VisibilityScopeType.Self;
    public bool IncludeSelf { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 100;
    public ICollection<UserVisibilityPolicy> UserAssignments { get; set; } = [];
}

public sealed class UserVisibilityPolicy : Entity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public long VisibilityPolicyId { get; set; }
    public VisibilityPolicy VisibilityPolicy { get; set; } = null!;
}

public sealed class AccessControlAuditLog : Entity
{
    public string TraceId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string Result { get; set; } = "Success";
    public string Source { get; set; } = "AccessControl";
    public string? BranchCode { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? Reason { get; set; }
    public string? FailureReason { get; set; }
    public long? PerformedByUserId { get; set; }
    public string? PerformedByUserEmail { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? ChangedFieldsJson { get; set; }
}
