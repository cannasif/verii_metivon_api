using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Modules.Pricing.Application.Services;
namespace verii_metivon_api.Modules.Pricing.Api;
[ApiController,Authorize,Route("api/pricing")]
public sealed class PricingController(IPricingService service):ControllerBase
{
 [HttpGet("price-lists")]public async Task<IActionResult>List([FromQuery]PricingQuery q,CancellationToken ct){var r=await service.GetPagedAsync(q,ct);return StatusCode(r.StatusCode,r);}
 [HttpGet("price-lists/{id:long}")]public async Task<IActionResult>Get(long id,CancellationToken ct){var r=await service.GetAsync(id,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("price-lists")]public async Task<IActionResult>Create(SavePriceListRequest x,CancellationToken ct){var r=await service.CreateAsync(x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPut("price-lists/{id:long}")]public async Task<IActionResult>Update(long id,SavePriceListRequest x,CancellationToken ct){var r=await service.UpdateAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
 [HttpDelete("price-lists/{id:long}")]public async Task<IActionResult>Delete(long id,CancellationToken ct){var r=await service.DeleteAsync(id,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("price-lists/{id:long}/lines")]public async Task<IActionResult>Line(long id,SavePriceListLineRequest x,CancellationToken ct){var r=await service.AddLineAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("calculate")]public async Task<IActionResult>Calculate(PriceRequest x,CancellationToken ct){var r=await service.CalculateAsync(x,ct);return StatusCode(r.StatusCode,r);}
}
