using verii_metivon_api.Modules.BusinessPartners.Application.Services;

namespace verii_metivon_api.Modules.BusinessPartners;

public static class BusinessPartnersModule
{
    public static IServiceCollection AddBusinessPartnersModule(this IServiceCollection services)
    {
        services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
        return services;
    }
}
