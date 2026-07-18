using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.Products.Application.Parameters;

namespace verii_metivon_api.Modules.Products.Api;

[ApiController,Authorize,Route("api/parameters/products")]
public sealed class ProductParametersController(IProductParameterService service):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]long? branchId,CancellationToken ct)=>
        Ok(ApiResponse<ProductParametersDto>.Ok(await service.GetAsync(branchId,ct)));

    [HttpPut,Authorize(Roles="Admin")]
    public async Task<IActionResult> Save(SaveProductParametersRequest request,CancellationToken ct)
    {
        try{return Ok(ApiResponse<ProductParametersDto>.Ok(await service.SaveAsync(request,ct)));}
        catch(ArgumentException error){return BadRequest(ApiResponse<object>.Error(error.Message,400));}
    }
}
