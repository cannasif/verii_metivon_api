using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Modules.Products.Application.Services;

namespace verii_metivon_api.Modules.Products.Api;

[ApiController, Authorize, Route("api/products")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    private string? Culture => Request.Headers.AcceptLanguage.FirstOrDefault();
    [HttpGet] public async Task<IActionResult> GetPaged([FromQuery] ProductListQuery query, CancellationToken ct) => Ok(await service.GetPagedAsync(query, Culture, ct));
    [HttpGet("definitions")] public async Task<IActionResult> GetDefinitions(CancellationToken ct) => Ok(await service.GetDefinitionsAsync(Culture, ct));
    [HttpPost] public async Task<IActionResult> Create(SaveProductRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(request, Culture, ct); return StatusCode(response.StatusCode, response);
    }
    [HttpGet("{id:long}")] public async Task<IActionResult> Get(long id,CancellationToken ct){var response=await service.GetByIdAsync(id,ct);return StatusCode(response.StatusCode,response);}
    [HttpPut("{id:long}")] public async Task<IActionResult> Update(long id,SaveProductRequest request,CancellationToken ct){var response=await service.UpdateAsync(id,request,Culture,ct);return StatusCode(response.StatusCode,response);}
    [HttpDelete("{id:long}")] public async Task<IActionResult> Delete(long id,CancellationToken ct){var response=await service.DeleteAsync(id,ct);return StatusCode(response.StatusCode,response);}
    [HttpGet("definition-management/{kind}")] public async Task<IActionResult> GetManagedDefinitions(string kind,[FromQuery] ProductDefinitionListQuery query,CancellationToken ct)=>Ok(await service.GetManagedDefinitionsAsync(kind,query,Culture,ct));
    [HttpPost("definition-management/{kind}")] public async Task<IActionResult> CreateDefinition(string kind,SaveProductDefinitionRequest request,CancellationToken ct){var response=await service.CreateDefinitionAsync(kind,request,Culture,ct);return StatusCode(response.StatusCode,response);}
    [HttpPut("definition-management/{kind}/{id:long}")] public async Task<IActionResult> UpdateDefinition(string kind,long id,SaveProductDefinitionRequest request,CancellationToken ct){var response=await service.UpdateDefinitionAsync(kind,id,request,Culture,ct);return StatusCode(response.StatusCode,response);}
    [HttpDelete("definition-management/{kind}/{id:long}")] public async Task<IActionResult> DeleteDefinition(string kind,long id,CancellationToken ct){var response=await service.DeleteDefinitionAsync(kind,id,Culture,ct);return StatusCode(response.StatusCode,response);}
}
