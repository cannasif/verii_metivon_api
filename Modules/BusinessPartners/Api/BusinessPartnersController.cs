using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.BusinessPartners.Application.Services;

namespace verii_metivon_api.Modules.BusinessPartners.Api;

[ApiController, Authorize, Route("api/business-partners")]
public sealed class BusinessPartnersController(IBusinessPartnerService service) : ControllerBase
{
    private string? Culture => Request.Headers.AcceptLanguage.FirstOrDefault();
    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] BusinessPartnerListQuery query, CancellationToken ct) => Ok(await service.GetAllAsync(query, Culture, ct));
    [HttpGet("definitions")] public async Task<IActionResult> GetDefinitions(CancellationToken ct) => Ok(await service.GetDefinitionsAsync(Culture, ct));
    [HttpGet("{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var response=await service.GetByIdAsync(id,Culture,ct);return StatusCode(response.StatusCode,response);
    }
    [HttpPost] public async Task<IActionResult> Create(CreateBusinessPartnerRequest request, CancellationToken ct)
    {
        var response = await service.CreateAsync(request, Culture, ct);
        return StatusCode(response.StatusCode, response);
    }
    [HttpPut("{id:long}")] public async Task<IActionResult> Update(long id, CreateBusinessPartnerRequest request, CancellationToken ct)
    {
        var response=await service.UpdateAsync(id,request,Culture,ct);return StatusCode(response.StatusCode,response);
    }
    [HttpDelete("{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var response=await service.DeleteAsync(id,Culture,ct);return StatusCode(response.StatusCode,response);
    }
    [HttpGet("definition-management/{kind}")]
    public async Task<IActionResult> GetManagedDefinitions(string kind, [FromQuery] DefinitionListQuery query, CancellationToken ct) => Ok(await service.GetManagedDefinitionsAsync(kind, query, Culture, ct));
    [HttpPost("definition-management/{kind}")]
    public async Task<IActionResult> CreateDefinition(string kind, SaveDefinitionRequest request, CancellationToken ct)
    {
        var response = await service.CreateDefinitionAsync(kind, request, Culture, ct);
        return StatusCode(response.StatusCode, response);
    }
    [HttpPut("definition-management/{kind}/{id:long}")]
    public async Task<IActionResult> UpdateDefinition(string kind, long id, SaveDefinitionRequest request, CancellationToken ct)
    {
        var response = await service.UpdateDefinitionAsync(kind, id, request, Culture, ct);
        return StatusCode(response.StatusCode, response);
    }
    [HttpDelete("definition-management/{kind}/{id:long}")]
    public async Task<IActionResult> DeleteDefinition(string kind, long id, CancellationToken ct)
    {
        var response = await service.DeleteDefinitionAsync(kind, id, Culture, ct);
        return StatusCode(response.StatusCode, response);
    }
}
