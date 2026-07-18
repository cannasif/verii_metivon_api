using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.Traceability.Application.Parameters;

namespace verii_metivon_api.Modules.Traceability.Api;

[ApiController,Authorize,Route("api/parameters/inventory-traceability")]
public sealed class InventoryTraceabilityParametersController(IInventoryTraceabilityParameterService service):ControllerBase
{
    [HttpGet]public async Task<IActionResult> Get([FromQuery]long? branchId,[FromQuery]long? warehouseId,CancellationToken ct)=>Ok(ApiResponse<InventoryTraceabilityParametersDto>.Ok(await service.GetAsync(branchId,warehouseId,ct)));
    [HttpPut,Authorize(Roles="Admin")]public async Task<IActionResult> Save(SaveInventoryTraceabilityParametersRequest r,CancellationToken ct){try{return Ok(ApiResponse<InventoryTraceabilityParametersDto>.Ok(await service.SaveAsync(r,ct)));}catch(ArgumentException e){return BadRequest(ApiResponse<object>.Error(e.Message,400));}}
}
