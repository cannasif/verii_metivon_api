namespace verii_metivon_api.Modules.Organization.Application.Dtos;

public sealed record BranchOptionDto(long Id, string Code, string Name, bool IsDefault);

public sealed record BranchRow(
    long Id,
    string Code,
    string Name,
    bool IsDefault,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record SaveBranchRequest(string Code, string Name, bool IsDefault, bool IsActive);

