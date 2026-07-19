using verii_metivon_api.Core.Paging;

namespace verii_metivon_api.Modules.Organization.Application.Queries;

public sealed class BranchQuery : PagedQuery
{
    public bool? IsActive { get; init; }
}

