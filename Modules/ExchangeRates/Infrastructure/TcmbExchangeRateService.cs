using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using verii_metivon_api.Modules.ExchangeRates.Application.Dtos;
using verii_metivon_api.Modules.ExchangeRates.Application.Services;

namespace verii_metivon_api.Modules.ExchangeRates.Infrastructure;

public sealed class TcmbExchangeRateService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<TcmbExchangeRateService> logger) : IExchangeRateService
{
    private const string FreshCacheKey = "exchange-rates:tcmb:fresh";
    private const string LastSuccessfulCacheKey = "exchange-rates:tcmb:last-successful";
    private static readonly TimeSpan FreshDuration = TimeSpan.FromMinutes(15);
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);
    private static readonly string[] HourlyPublicationTimes = ["1100", "1200", "1400", "1500", "1000", "1300"];

    public async Task<ExchangeRateSnapshotDto> GetLatestAsync(
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceRefresh &&
            cache.TryGetValue(FreshCacheKey, out ExchangeRateSnapshotDto? fresh) &&
            fresh is not null)
            return fresh;

        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            if (!forceRefresh &&
                cache.TryGetValue(FreshCacheKey, out fresh) &&
                fresh is not null)
                return fresh;

            try
            {
                var snapshot = await FetchAsync(cancellationToken);
                cache.Set(FreshCacheKey, snapshot, FreshDuration);
                cache.Set(LastSuccessfulCacheKey, snapshot);
                return snapshot;
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or FormatException)
            {
                logger.LogWarning(exception, "TCMB exchange rates could not be refreshed.");
                if (cache.TryGetValue(LastSuccessfulCacheKey, out ExchangeRateSnapshotDto? stale) && stale is not null)
                    return stale with { IsStale = true };

                throw new ExchangeRateUnavailableException("TCMB exchange rates are temporarily unavailable.", exception);
            }
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private async Task<ExchangeRateSnapshotDto> FetchAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(ExchangeRatesModule.TcmbClientName);
        await using var stream = await client.GetStreamAsync("kurlar/today.xml", cancellationToken);
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
        var root = document.Root ?? throw new FormatException("TCMB response does not contain a root element.");
        var dateText = root.Attribute("Tarih")?.Value
            ?? throw new FormatException("TCMB response does not contain a rate date.");
        var rateDate = DateOnly.ParseExact(dateText, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        var currencyRates = root.Elements("Currency")
            .Select(ParseCurrency)
            .Where(rate => rate is not null)
            .Cast<ExchangeRateItemDto>()
            .ToArray();

        if (currencyRates.Length == 0)
            throw new FormatException("TCMB response does not contain usable exchange rates.");

        var preciousMetalRates = await FetchLatestPreciousMetalsAsync(client, rateDate, cancellationToken);
        var rates = currencyRates
            .Concat(preciousMetalRates)
            .OrderBy(rate => rate.InstrumentType, StringComparer.Ordinal)
            .ThenBy(rate => rate.CurrencyCode, StringComparer.Ordinal)
            .ToArray();

        return new ExchangeRateSnapshotDto(
            preciousMetalRates.Count > 0 ? "TCMB Daily + Hourly" : "TCMB Daily",
            rateDate,
            DateTimeOffset.UtcNow,
            false,
            rates);
    }

    private static ExchangeRateItemDto? ParseCurrency(XElement element)
    {
        var code = element.Attribute("CurrencyCode")?.Value?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code)) return null;

        if (!decimal.TryParse(element.Element("ForexBuying")?.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var buying) ||
            !decimal.TryParse(element.Element("ForexSelling")?.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var selling))
            return null;
        var unit = int.TryParse(element.Attribute("Unit")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedUnit)
            ? Math.Max(1, parsedUnit)
            : 1;

        return new ExchangeRateItemDto(
            code,
            unit,
            buying,
            selling,
            ParseOptionalDecimal(element.Element("BanknoteBuying")?.Value),
            ParseOptionalDecimal(element.Element("BanknoteSelling")?.Value),
            "Currency",
            element.Element("Isim")?.Value?.Trim(),
            null);
    }

    private static async Task<IReadOnlyList<ExchangeRateItemDto>> FetchLatestPreciousMetalsAsync(
        HttpClient client,
        DateOnly dailyRateDate,
        CancellationToken cancellationToken)
    {
        for (var dayOffset = 0; dayOffset < 7; dayOffset++)
        {
            var date = dailyRateDate.AddDays(-dayOffset);
            foreach (var publicationTime in HourlyPublicationTimes)
            {
                var path = $"reeskontkur/{date:yyyyMM}/{date:ddMMyyyy}-{publicationTime}.xml";
                try
                {
                    using var response = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    if (!response.IsSuccessStatusCode) continue;
                    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
                    var metals = document.Descendants("kur")
                        .Select(element => ParsePreciousMetal(element, date))
                        .Where(rate => rate is not null)
                        .Cast<ExchangeRateItemDto>()
                        .OrderBy(rate => rate.CurrencyCode, StringComparer.Ordinal)
                        .ToArray();
                    if (metals.Length > 0) return metals;
                }
                catch (HttpRequestException)
                {
                    // Try the next official publication slot or previous business day.
                }
                catch (FormatException)
                {
                    // A malformed hourly file must not make daily exchange rates unavailable.
                }
                catch (System.Xml.XmlException)
                {
                    // Try another official publication if an hourly response is incomplete.
                }
                catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // A precious-metal timeout must not hide otherwise valid daily rates.
                }
            }
        }

        return [];
    }

    private static ExchangeRateItemDto? ParsePreciousMetal(XElement element, DateOnly rateDate)
    {
        var code = element.Element("doviz_cinsi")?.Value?.Trim().ToUpperInvariant();
        if (code is not ("XAU" or "XAS")) return null;
        if (!decimal.TryParse(element.Element("alis")?.Value, NumberStyles.Number, CultureInfo.GetCultureInfo("tr-TR"), out var buying))
            return null;
        var unit = int.TryParse(element.Element("birim")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedUnit)
            ? Math.Max(1, parsedUnit)
            : 1;

        return new ExchangeRateItemDto(
            code,
            unit,
            buying,
            null,
            null,
            null,
            "PreciousMetal",
            code == "XAU" ? "24 Ayar Altın" : "Saf (Has) Altın",
            rateDate);
    }

    private static decimal? ParseOptionalDecimal(string? value) =>
        decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
}

public sealed class ExchangeRateUnavailableException(string message, Exception innerException)
    : Exception(message, innerException);
