using verii_metivon_api.Modules.TradeOperations.Application.Services;
namespace verii_metivon_api.Modules.TradeOperations;
public static class TradeOperationsModule{public static IServiceCollection AddTradeOperationsModule(this IServiceCollection services){services.AddScoped<ITradeDossierService,TradeDossierService>();return services;}}
