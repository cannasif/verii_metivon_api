using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;

namespace verii_metivon_api.Modules.BusinessPartners.Application.Services;

public interface IBusinessPartnerService
{
    Task<ApiResponse<PagedResult<BusinessPartnerItem>>> GetAllAsync(BusinessPartnerListQuery query, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<BusinessPartnerDefinitions>> GetDefinitionsAsync(string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<BusinessPartnerDetail>> GetByIdAsync(long id, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<object>> CreateAsync(CreateBusinessPartnerRequest request, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<object>> UpdateAsync(long id, CreateBusinessPartnerRequest request, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<PagedResult<ManagedDefinitionItem>>> GetManagedDefinitionsAsync(string kind, DefinitionListQuery query, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<object>> CreateDefinitionAsync(string kind, SaveDefinitionRequest request, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<object>> UpdateDefinitionAsync(string kind, long id, SaveDefinitionRequest request, string? culture, CancellationToken cancellationToken);
    Task<ApiResponse<object>> DeleteDefinitionAsync(string kind, long id, string? culture, CancellationToken cancellationToken);
}
