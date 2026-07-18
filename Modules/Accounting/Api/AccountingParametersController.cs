using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using verii_metivon_api.Core.Auth;using verii_metivon_api.Modules.Accounting.Application.Parameters;
namespace verii_metivon_api.Modules.Accounting.Api;
[ApiController,Authorize,Route("api/parameters/accounting")]
public sealed class AccountingParametersController(IAccountingParameterService service):ControllerBase
{
 [HttpGet]public async Task<IActionResult>Get([FromQuery]long?branchId,[FromQuery]long?warehouseId,CancellationToken ct)=>Ok(ApiResponse<AccountingParametersDto>.Ok(await service.GetAsync(branchId,warehouseId,ct)));
 [HttpPut,Authorize(Roles="Admin")]public async Task<IActionResult>Save(SaveAccountingParametersRequest r,CancellationToken ct){try{return Ok(ApiResponse<AccountingParametersDto>.Ok(await service.SaveAsync(r,ct)));}catch(ArgumentException e){return BadRequest(ApiResponse<object>.Error(e.Message,400));}}
}
