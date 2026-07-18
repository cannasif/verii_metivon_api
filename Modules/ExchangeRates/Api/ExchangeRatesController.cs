using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.ExchangeRates.Application.Dtos;
using verii_metivon_api.Modules.ExchangeRates.Application.Services;
using verii_metivon_api.Modules.ExchangeRates.Infrastructure;

namespace verii_metivon_api.Modules.ExchangeRates.Api;

[ApiController]
[Authorize]
[Route("api/exchange-rates")]
public sealed class ExchangeRatesController(IExchangeRateService service) : ControllerBase
{
    [HttpGet("latest")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeRateSnapshotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetLatest(
        [FromQuery] bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Response.Headers.CacheControl = "no-store";
            return Ok(ApiResponse<ExchangeRateSnapshotDto>.Ok(
                await service.GetLatestAsync(forceRefresh, cancellationToken)));
        }
        catch (ExchangeRateUnavailableException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                ApiResponse<object>.Error("Exchange rates are temporarily unavailable.", StatusCodes.Status503ServiceUnavailable));
        }
    }
}
