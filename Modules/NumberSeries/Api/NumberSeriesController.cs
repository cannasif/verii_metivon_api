using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.NumberSeries.Application;

namespace verii_metivon_api.Modules.NumberSeries.Api;

[ApiController,Authorize,Route("api/number-series")]
public sealed class NumberSeriesController(INumberSeriesService service) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get([FromQuery] NumberSeriesQuery query,CancellationToken ct){var result=await service.GetPagedAsync(query,ct);return StatusCode(result.StatusCode,result);}
    [HttpGet("usages")] public async Task<IActionResult> Usages([FromQuery] NumberSeriesUsageQuery query,CancellationToken ct){var result=await service.GetUsagePagedAsync(query,ct);return StatusCode(result.StatusCode,result);}
    [HttpPost] public async Task<IActionResult> Create(SaveNumberSeriesRequest request,CancellationToken ct){var result=await service.SaveAsync(null,request,ct);return StatusCode(result.StatusCode,result);}
    [HttpPost("{id:long}/update")] public async Task<IActionResult> Update(long id,SaveNumberSeriesRequest request,CancellationToken ct){var result=await service.SaveAsync(id,request,ct);return StatusCode(result.StatusCode,result);}
    [HttpPost("{id:long}/delete")] public async Task<IActionResult> Delete(long id,CancellationToken ct){var result=await service.DeleteAsync(id,ct);return StatusCode(result.StatusCode,result);}
    [HttpGet("available")] public async Task<IActionResult> Available([FromQuery]string module,[FromQuery]string reference,[FromQuery]long? branchId,[FromQuery]long? warehouseId,CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<NumberSeriesRow>>.Ok(await service.GetAvailableAsync(module,reference,branchId,warehouseId,ct)));
    [HttpPost("reserve")] public async Task<IActionResult> Reserve(ReserveNumberRequest request,CancellationToken ct)=>Ok(ApiResponse<ReservedNumber>.Ok(await service.ReserveAsync(request,ct)));
    [HttpPost("usages/{usageId:long}/cancel")] public async Task<IActionResult> Cancel(long usageId,CancelNumberUsageRequest request,CancellationToken ct){var result=await service.CancelAsync(usageId,request.Reason,ct);return StatusCode(result.StatusCode,result);}
    [HttpPost("usages/cleanup-expired")] public async Task<IActionResult> CleanupExpired(CleanupNumberReservationsRequest request,CancellationToken ct){var result=await service.CleanupExpiredAsync(request,ct);return StatusCode(result.StatusCode,result);}
}
public sealed record CancelNumberUsageRequest(string Reason);
