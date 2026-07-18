using System.Net.Http.Headers;
using verii_metivon_api.Modules.ExchangeRates.Application.Services;
using verii_metivon_api.Modules.ExchangeRates.Infrastructure;

namespace verii_metivon_api.Modules.ExchangeRates;

public static class ExchangeRatesModule
{
    internal const string TcmbClientName = "TcmbExchangeRates";

    public static IServiceCollection AddExchangeRatesModule(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient(TcmbClientName, client =>
        {
            client.BaseAddress = new Uri("https://www.tcmb.gov.tr/");
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("V3RII-ERP/1.0");
        });
        services.AddSingleton<IExchangeRateService, TcmbExchangeRateService>();
        return services;
    }
}
