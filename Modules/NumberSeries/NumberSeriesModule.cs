using verii_metivon_api.Modules.NumberSeries.Application;
namespace verii_metivon_api.Modules.NumberSeries;
public static class NumberSeriesModule{public static IServiceCollection AddNumberSeriesModule(this IServiceCollection services){services.AddScoped<INumberSeriesService,NumberSeriesService>();return services;}}
