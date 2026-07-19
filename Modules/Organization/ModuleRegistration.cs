using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using verii_metivon_api.Core.Localization;
using verii_metivon_api.Core.Modularity;
using verii_metivon_api.Modules.Organization.Application.Abstractions;
using verii_metivon_api.Modules.Organization.Application.Mappings;
using verii_metivon_api.Modules.Organization.Application.Services;
using verii_metivon_api.Modules.Organization.Application.Validators;
using verii_metivon_api.Modules.Organization.Localization;

namespace verii_metivon_api.Modules.Organization;

public static class ModuleRegistration
{
    public static IServiceCollection AddOrganizationModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(OrganizationMappingProfile));
        services.AddValidatorsFromAssemblyContaining<SaveBranchRequestValidator>();
        services.AddSingleton<IModuleManifestProvider, OrganizationModuleManifest>();
        services.AddSingleton<IModuleLocalizer<OrganizationLocalizationResource>, JsonModuleLocalizer<OrganizationLocalizationResource>>();
        services.AddScoped<IBranchService, BranchService>();
        return services;
    }
}
