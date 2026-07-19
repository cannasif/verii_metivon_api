using AutoMapper;
using verii_metivon_api.Modules.Organization.Application.Dtos;

namespace verii_metivon_api.Modules.Organization.Application.Mappings;

public sealed class OrganizationMappingProfile : Profile
{
    public OrganizationMappingProfile()
    {
        CreateMap<Branch, BranchRow>();
        CreateMap<Branch, BranchOptionDto>();
    }
}

