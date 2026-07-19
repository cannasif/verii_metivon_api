namespace verii_metivon_api.Core.Auth;

using verii_metivon_api.Core.Paging;

public sealed record LoginRequest(string Email, string Password, bool RememberMe);
public sealed record RefreshRequest(string RefreshToken);
public sealed record LoginResult(string Token, string RefreshToken, DateTime RefreshTokenExpiresAt, long UserId, string SessionId, bool RememberMe);
public sealed record MyPermissionsResult(long UserId, string RoleTitle, bool IsSystemAdmin,
    IReadOnlyList<string> PermissionGroups, IReadOnlyList<string> PermissionCodes);
public sealed record AppBootstrapUser(long Id, string Email, string Name);
public sealed record AppBootstrapResult(AppBootstrapUser User, MyPermissionsResult Permissions, object SystemSettings);
public sealed record DefinitionItem(long Id, string Code, string Name, bool IsDefault);
public sealed record ManagedDefinitionItem(long Id, string Code, string Name, string? Description, bool IsActive,
    bool IsDefault, int DisplayOrder, int? DueDays, int? DiscountDays, decimal? DiscountRate,
    string? IsoCode, string? Symbol, int? DecimalPlaces, bool? IsTaxExempt);
public sealed record SaveDefinitionRequest(string Code, string Name, string? Description, bool IsActive,
    bool IsDefault, int DisplayOrder, int? DueDays, int? DiscountDays, decimal? DiscountRate,
    string? IsoCode, string? Symbol, int? DecimalPlaces, bool? IsTaxExempt);
public sealed class DefinitionListQuery : PagedQuery
{
    public bool? IsActive { get; init; }
}
public sealed record BusinessPartnerDefinitions(
    IReadOnlyList<DefinitionItem> PartnerTypes,
    IReadOnlyList<DefinitionItem> CustomerGroups,
    IReadOnlyList<DefinitionItem> PaymentTerms,
    IReadOnlyList<DefinitionItem> Currencies,
    IReadOnlyList<DefinitionItem> TaxGroups);
public sealed record CreateBusinessPartnerRequest(
    string? Code, string Name, string? LegalName, long BranchId, long? BusinessPartnerTypeId,
    long? CustomerGroupId, long? PaymentTermId, long? CurrencyId, long? TaxGroupId,
    string? TaxOffice, string? TaxNumber, string? NationalIdentityNumber,
    string? Email, string? Phone, string? MobilePhone, string? Website,
    decimal? CreditLimit, bool? HasUnlimitedCredit, string? Notes);
public sealed record BusinessPartnerDetail(
    long Id,string Code,string Name,string? LegalName,long BranchId,long BusinessPartnerTypeId,long? CustomerGroupId,
    long PaymentTermId,long CurrencyId,long TaxGroupId,string? TaxOffice,string? TaxNumber,string? NationalIdentityNumber,
    string? Email,string? Phone,string? MobilePhone,string? Website,decimal CreditLimit,bool HasUnlimitedCredit,bool IsActive,string? Notes);
public sealed record BusinessPartnerItem(
    long Id, string Code, string Name, string PartnerType, string? CustomerGroup,
    string PaymentTerm, string Currency, string TaxGroup, decimal CreditLimit,
    bool HasUnlimitedCredit, bool IsActive);
public sealed class BusinessPartnerListQuery : PagedQuery
{
    public bool? IsActive { get; init; }
}

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string ExceptionMessage { get; init; } = string.Empty;
    public string? ErrorCode { get; init; }
    public T? Data { get; init; }
    public int StatusCode { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public static ApiResponse<T> Ok(T data, string message = "Success") => new() { Success = true, Data = data, Message = message, StatusCode = 200 };
    public static ApiResponse<T> Error(string message, int statusCode, string? errorCode = null) => new()
    {
        Success = false,
        Message = message,
        ErrorCode = errorCode ?? DefaultErrorCode(statusCode),
        StatusCode = statusCode
    };

    private static string DefaultErrorCode(int statusCode) => statusCode switch
    {
        400 => "API_BAD_REQUEST",
        401 => "API_UNAUTHORIZED",
        403 => "API_FORBIDDEN",
        404 => "API_NOT_FOUND",
        409 => "API_CONFLICT",
        >= 500 => "API_SERVER_ERROR",
        _ => "API_ERROR"
    };
}
