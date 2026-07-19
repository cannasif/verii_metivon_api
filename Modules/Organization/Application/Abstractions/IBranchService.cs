using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Modules.Organization.Application.Dtos;
using verii_metivon_api.Modules.Organization.Application.Queries;

namespace verii_metivon_api.Modules.Organization.Application.Abstractions;

public interface IBranchService
{
    Task<IReadOnlyList<BranchOptionDto>> GetActiveAsync(CancellationToken cancellationToken);
    Task<ApiResponse<PagedResult<BranchRow>>> GetPagedAsync(BranchQuery query, CancellationToken cancellationToken);
    Task<ApiResponse<BranchRow>> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<ApiResponse<BranchRow>> SaveAsync(long? id, SaveBranchRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<object>> DeleteAsync(long id, CancellationToken cancellationToken);
}

