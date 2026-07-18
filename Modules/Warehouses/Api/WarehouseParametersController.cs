using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.Warehouses.Application.Parameters;
namespace verii_metivon_api.Modules.Warehouses.Api;
[ApiController,Authorize,Route("api/parameters/warehouses")]
public sealed class WarehouseParametersController(IWarehouseParameterService service):ControllerBase{
 [HttpGet]public async Task<IActionResult>Get([FromQuery]long?branchId,[FromQuery]long?warehouseId,CancellationToken ct)=>Ok(ApiResponse<WarehouseParametersDto>.Ok(await service.GetAsync(branchId,warehouseId,ct)));
 [HttpPut,Authorize(Roles="Admin")]public async Task<IActionResult>Save(SaveWarehouseParametersRequest request,CancellationToken ct){try{return Ok(ApiResponse<WarehouseParametersDto>.Ok(await service.SaveAsync(request,ct)));}catch(ArgumentException e){return BadRequest(ApiResponse<object>.Error(e.Message,400));}}
}
