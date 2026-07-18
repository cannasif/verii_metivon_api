using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Modules.Accounting.Application.Definitions;

namespace verii_metivon_api.Modules.Accounting.Api;

[ApiController, Authorize, Route("api/accounting/definitions")]
public sealed class AccountingDefinitionsController(IAccountingDefinitionService service) : ControllerBase
{
    [HttpGet("fiscal-periods")] public async Task<IActionResult> FiscalPeriods([FromQuery] FiscalPeriodQuery query, CancellationToken ct) { var result = await service.GetFiscalPeriodsAsync(query, ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("fiscal-periods/{id:long}")] public async Task<IActionResult> FiscalPeriod(long id, CancellationToken ct) { var result = await service.GetFiscalPeriodAsync(id, ct); return StatusCode(result.StatusCode, result); }
    [HttpPost("fiscal-periods")] public async Task<IActionResult> CreateFiscalPeriod(SaveFiscalPeriodRequest request, CancellationToken ct) { var result = await service.CreateFiscalPeriodAsync(request, ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("fiscal-periods/{id:long}")] public async Task<IActionResult> UpdateFiscalPeriod(long id, SaveFiscalPeriodRequest request, CancellationToken ct) { var result = await service.UpdateFiscalPeriodAsync(id, request, ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("fiscal-periods/{id:long}")] public async Task<IActionResult> DeleteFiscalPeriod(long id, CancellationToken ct) { var result = await service.DeleteFiscalPeriodAsync(id, ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("inventory-posting-profiles")] public async Task<IActionResult> PostingProfiles([FromQuery] InventoryPostingProfileQuery query, CancellationToken ct) { var result = await service.GetPostingProfilesAsync(query, ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("inventory-posting-profiles/{id:long}")] public async Task<IActionResult> PostingProfile(long id, CancellationToken ct) { var result = await service.GetPostingProfileAsync(id, ct); return StatusCode(result.StatusCode, result); }
    [HttpPost("inventory-posting-profiles")] public async Task<IActionResult> CreatePostingProfile(SaveInventoryPostingProfileRequest request, CancellationToken ct) { var result = await service.CreatePostingProfileAsync(request, ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("inventory-posting-profiles/{id:long}")] public async Task<IActionResult> UpdatePostingProfile(long id, SaveInventoryPostingProfileRequest request, CancellationToken ct) { var result = await service.UpdatePostingProfileAsync(id, request, ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("inventory-posting-profiles/{id:long}")] public async Task<IActionResult> DeletePostingProfile(long id, CancellationToken ct) { var result = await service.DeletePostingProfileAsync(id, ct); return StatusCode(result.StatusCode, result); }
}
