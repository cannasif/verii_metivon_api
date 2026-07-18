using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.Receiving.Application.Parameters;
namespace verii_metivon_api.Modules.Receiving.Api;
[ApiController,Authorize,Route("api/parameters/receiving")]
public sealed class ReceivingParametersController(IReceivingParameterService service):ControllerBase
{
    [HttpGet]public async Task<IActionResult>Get([FromQuery]long? branchId,[FromQuery]long? warehouseId,CancellationToken ct)=>Ok(ApiResponse<ReceivingParametersDto>.Ok(await service.GetAsync(branchId,warehouseId,ct)));
    [HttpPut,Authorize(Roles="Admin")]public async Task<IActionResult>Save(SaveReceivingParametersRequest request,CancellationToken ct){try{return Ok(ApiResponse<ReceivingParametersDto>.Ok(await service.SaveAsync(request,ct)));}catch(ArgumentException error){return BadRequest(ApiResponse<object>.Error(error.Message,400));}}
}
