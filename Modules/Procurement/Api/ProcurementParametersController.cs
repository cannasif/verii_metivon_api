using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.Procurement.Application.Parameters;
namespace verii_metivon_api.Modules.Procurement.Api;
[ApiController,Authorize,Route("api/parameters/procurement")]
public sealed class ProcurementParametersController(IProcurementParameterService service):ControllerBase
{
 [HttpGet]public async Task<IActionResult>Get([FromQuery]long?branchId,[FromQuery]long?warehouseId,CancellationToken ct)=>Ok(ApiResponse<ProcurementParametersDto>.Ok(await service.GetAsync(branchId,warehouseId,ct)));
 [HttpPut,Authorize(Roles="Admin")]public async Task<IActionResult>Save(SaveProcurementParametersRequest r,CancellationToken ct){try{return Ok(ApiResponse<ProcurementParametersDto>.Ok(await service.SaveAsync(r,ct)));}catch(ArgumentException e){return BadRequest(ApiResponse<object>.Error(e.Message,400));}}
}
