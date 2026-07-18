using verii_metivon_api.Modules.ExchangeRates.Application.Dtos;

namespace verii_metivon_api.Modules.ExchangeRates.Application.Services;

public interface IExchangeRateService
{
    Task<ExchangeRateSnapshotDto> GetLatestAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default);
}
