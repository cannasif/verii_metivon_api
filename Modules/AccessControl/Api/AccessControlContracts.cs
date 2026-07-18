using System.ComponentModel.DataAnnotations;
using verii_metivon_api.Core.Paging;

namespace verii_metivon_api.Modules.AccessControl.Api;

public abstract record AuditedDto(long Id, DateTime CreatedDate, DateTime? UpdatedDate, DateTime? DeletedDate, bool IsDeleted);
public sealed record PermissionDefinitionDto(long Id, DateTime CreatedDate, DateTime? UpdatedDate, DateTime? DeletedDate, bool IsDeleted,
    string Code, string Name, string? Description, bool IsActive, bool AvailableOnWeb, bool AvailableOnMobile, string Module, string Resource, string Action);
public sealed record SavePermissionDefinitionRequest([Required, MaxLength(180)] string Code, [Required, MaxLength(180)] string Name,
    [MaxLength(500)] string? Description, bool IsActive = true, bool AvailableOnWeb = true, bool AvailableOnMobile = false);
public sealed record UpdatePermissionDefinitionRequest(string? Code, string? Name, string? Description, bool? IsActive, bool? AvailableOnWeb, bool? AvailableOnMobile);
public sealed record SyncPermissionDefinitionItemDto(string Code, string? Name, string? Description, bool IsActive, bool AvailableOnWeb, bool AvailableOnMobile);
public sealed record SyncPermissionDefinitionsRequest(IReadOnlyList<SyncPermissionDefinitionItemDto> Items, bool ReactivateSoftDeleted = true,
    bool UpdateExistingNames = true, bool UpdateExistingDescriptions = true, bool UpdateExistingIsActive = true);
public sealed record PermissionDefinitionSyncResultDto(int CreatedCount, int UpdatedCount, int ReactivatedCount, int TotalProcessed);
public sealed class PermissionDefinitionQuery : PagedQuery { public bool? IsActive { get; init; } }

public sealed record PermissionGroupDto(long Id, DateTime CreatedDate, DateTime? UpdatedDate, DateTime? DeletedDate, bool IsDeleted,
    string Name, string? Description, bool IsSystemAdmin, bool IsActive, IReadOnlyList<long> PermissionDefinitionIds, IReadOnlyList<string> PermissionCodes,
    string Code, int Priority, bool IsSystem);
public sealed record CreatePermissionGroupRequest([Required, MaxLength(120)] string Name, [MaxLength(500)] string? Description,
    bool IsSystemAdmin, bool IsActive, IReadOnlyList<long> PermissionDefinitionIds, string? Code = null, int Priority = 100);
public sealed record UpdatePermissionGroupRequest(string? Name, string? Description, bool? IsSystemAdmin, bool? IsActive, string? Code = null, int? Priority = null);
public sealed record SetPermissionGroupPermissionsRequest(IReadOnlyList<long> PermissionDefinitionIds);
public sealed class PermissionGroupQuery : PagedQuery { public bool? IsActive { get; init; } }
public sealed record UserPermissionGroupDto(long UserId, IReadOnlyList<long> PermissionGroupIds, IReadOnlyList<string> PermissionGroupNames);
public sealed record SetUserPermissionGroupsRequest(IReadOnlyList<long> PermissionGroupIds);

public sealed record VisibilityPolicyDto(long Id, DateTime CreatedDate, DateTime? UpdatedDate, DateTime? DeletedDate, bool IsDeleted,
    string Code, string Name, string EntityType, string? Description, int ScopeType, bool IncludeSelf, bool IsActive, int Priority);
public sealed record CreateVisibilityPolicyRequest([Required, MaxLength(120)] string Code, [Required, MaxLength(150)] string Name,
    [Required, MaxLength(60)] string EntityType, [MaxLength(500)] string? Description, [Range(1, 6)] int ScopeType, bool IncludeSelf, bool IsActive, int Priority = 100);
public sealed record UpdateVisibilityPolicyRequest(string? Code, string? Name, string? EntityType, string? Description, int? ScopeType, bool? IncludeSelf, bool? IsActive, int? Priority = null);
public sealed class VisibilityPolicyQuery : PagedQuery { public bool? IsActive { get; init; } public string? EntityType { get; init; } }
public sealed record UserVisibilityPolicyDto(long Id, DateTime CreatedDate, DateTime? UpdatedDate, DateTime? DeletedDate, bool IsDeleted,
    long UserId, string UserDisplayName, long VisibilityPolicyId, string VisibilityPolicyName, string EntityType, int ScopeType);
public sealed record CreateUserVisibilityPolicyRequest(long UserId, long VisibilityPolicyId);
public sealed record UpdateUserVisibilityPolicyRequest(long? UserId, long? VisibilityPolicyId);
public sealed class UserVisibilityPolicyQuery : PagedQuery { public long? UserId { get; init; } }
public sealed record VisibilityPreviewPolicy(long PolicyId, string Code, string Name, int ScopeType, bool IncludeSelf);
public sealed record VisibilityPreviewUser(long UserId, string FullName, string? Email);
public sealed record VisibilityPreviewResult(long UserId, string EntityType, bool HasExplicitPolicy, bool IsUnrestricted,
    IReadOnlyList<long> VisibleUserIds, IReadOnlyList<VisibilityPreviewUser> VisibleUsers, IReadOnlyList<VisibilityPreviewPolicy> Policies,
    IReadOnlyList<long> ApprovalOverrideEntityIds, IReadOnlyList<object> ApprovalOverrideAuditEntries);
public sealed record ActionSimulationResult(string Action, bool Allowed, string Reason);
public sealed record VisibilityActionSimulationResult(long UserId, string EntityType, long EntityId, IReadOnlyList<ActionSimulationResult> Actions);

public sealed record AuditLogDto(long Id, string TraceId, string ActionType, string? EntityType, string? EntityId, string Result, string? Source,
    string? BranchCode, string? RequestPath, string? RequestMethod, string? Reason, string? FailureReason, long? PerformedByUserId,
    string? PerformedByUserEmail, string? OldValuesJson, string? NewValuesJson, string? ChangedFieldsJson, IReadOnlyList<object> ChangedFields, DateTime CreatedDate);
public sealed class AuditLogQuery : PagedQuery { }
