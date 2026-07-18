using verii_metivon_api.Modules.Parameters.Application;
namespace verii_metivon_api.Modules.Parameters;
public static class ParametersModule{public static IServiceCollection AddParametersModule(this IServiceCollection services){services.AddScoped<IParameterService,ParameterService>();return services;}}
