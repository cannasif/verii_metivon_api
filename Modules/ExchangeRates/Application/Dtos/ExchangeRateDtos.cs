namespace verii_metivon_api.Modules.ExchangeRates.Application.Dtos;

public sealed record ExchangeRateItemDto(
    string CurrencyCode,
    int Unit,
    decimal ForexBuying,
    decimal? ForexSelling,
    decimal? BanknoteBuying,
    decimal? BanknoteSelling,
    string InstrumentType = "Currency",
    string? DisplayName = null,
    DateOnly? InstrumentRateDate = null);

public sealed record ExchangeRateSnapshotDto(
    string Source,
    DateOnly RateDate,
    DateTimeOffset RetrievedAtUtc,
    bool IsStale,
    IReadOnlyList<ExchangeRateItemDto> Rates);
