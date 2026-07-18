using verii_metivon_api.Modules.Accounting.Application.Definitions;
using verii_metivon_api.Modules.Accounting.Application.Parameters;
using verii_metivon_api.Modules.Accounting.Application.Services;

namespace verii_metivon_api.Modules.Accounting;

public static class AccountingModule
{
    public static IServiceCollection AddAccountingModule(this IServiceCollection services)
    {
        services.AddScoped<IAccountingParameterService, AccountingParameterService>();
        services.AddScoped<IAccountingService, AccountingService>();
        services.AddScoped<IAccountingDefinitionService, AccountingDefinitionService>();
        return services;
    }
}
