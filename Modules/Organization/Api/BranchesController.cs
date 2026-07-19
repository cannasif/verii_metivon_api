using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Modules.AccessControl.Authorization;
using verii_metivon_api.Modules.Organization.Application.Abstractions;
using verii_metivon_api.Modules.Organization.Application.Dtos;
using verii_metivon_api.Modules.Organization.Application.Queries;

namespace verii_metivon_api.Modules.Organization.Api;

[ApiController,Authorize,Route("api/branches")]
public sealed class BranchesController(IBranchService service):ControllerBase
{
    [HttpGet,AllowAnonymous] public async Task<IActionResult> GetActive(CancellationToken ct)=>Ok(ApiResponse<IReadOnlyList<BranchOptionDto>>.Ok(await service.GetActiveAsync(ct)));
    [HttpPost("query"),PermissionAuthorize("organization.branches.view")] public async Task<IActionResult> Query(BranchQuery query,CancellationToken ct){var result=await service.GetPagedAsync(query,ct);return StatusCode(result.StatusCode,result);}
    [HttpGet("{id:long}"),PermissionAuthorize("organization.branches.view")] public async Task<IActionResult> GetById(long id,CancellationToken ct){var result=await service.GetByIdAsync(id,ct);return StatusCode(result.StatusCode,result);}
    [HttpPost,PermissionAuthorize("organization.branches.create")] public async Task<IActionResult> Create(SaveBranchRequest request,CancellationToken ct){var result=await service.SaveAsync(null,request,ct);return StatusCode(result.StatusCode,result);}
    [HttpPut("{id:long}"),PermissionAuthorize("organization.branches.update")] public async Task<IActionResult> Update(long id,SaveBranchRequest request,CancellationToken ct){var result=await service.SaveAsync(id,request,ct);return StatusCode(result.StatusCode,result);}
    [HttpDelete("{id:long}"),PermissionAuthorize("organization.branches.delete")] public async Task<IActionResult> Delete(long id,CancellationToken ct){var result=await service.DeleteAsync(id,ct);return StatusCode(result.StatusCode,result);}
}
