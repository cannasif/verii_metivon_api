using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.EDocuments.Application.Parameters;

namespace verii_metivon_api.Modules.EDocuments.Api;
[ApiController,Authorize,Route("api/parameters/e-documents")]
public sealed class EDocumentParametersController(IEDocumentParameterService service):ControllerBase
{
 [HttpGet]public async Task<IActionResult>Get([FromQuery]long branchId,CancellationToken ct){if(branchId<=0)return BadRequest(ApiResponse<object>.Error("Branch is required.",400));return Ok(ApiResponse<EDocumentParametersDto>.Ok(await service.GetAsync(branchId,ct)));}
 [HttpPut,Authorize(Roles="Admin")]public async Task<IActionResult>Save(SaveEDocumentParametersRequest r,CancellationToken ct){try{return Ok(ApiResponse<EDocumentParametersDto>.Ok(await service.SaveAsync(r,ct)));}catch(ArgumentException e){return BadRequest(ApiResponse<object>.Error(e.Message,400));}}
}
