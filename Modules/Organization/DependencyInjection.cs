using Microsoft.Extensions.DependencyInjection;
using verii_metivon_api.Modules.Organization.Application;

namespace verii_metivon_api.Modules.Organization;
public static class DependencyInjection
{
    public static IServiceCollection AddOrganizationModule(this IServiceCollection services)
    {
        services.AddScoped<IBranchService,BranchService>();
        return services;
    }
}
