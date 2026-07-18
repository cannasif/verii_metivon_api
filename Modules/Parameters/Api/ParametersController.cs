using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.Parameters.Application;
namespace verii_metivon_api.Modules.Parameters.Api;
[ApiController,Authorize,Route("api/parameters")]
public sealed class ParametersController(IParameterService service):ControllerBase
{
    [HttpGet("business-partners")]public async Task<IActionResult>Get([FromQuery]long? branchId,CancellationToken ct)=>Ok(ApiResponse<BusinessPartnerParametersDto>.Ok(await service.GetBusinessPartnerParametersAsync(branchId,ct)));
    [HttpPut("business-partners"),Authorize(Roles="Admin")]public async Task<IActionResult>Save(SaveBusinessPartnerParametersRequest request,CancellationToken ct){try{return Ok(ApiResponse<BusinessPartnerParametersDto>.Ok(await service.SaveBusinessPartnerParametersAsync(request,ct)));}catch(ArgumentException e){return BadRequest(ApiResponse<object>.Error(e.Message,400));}}
}
