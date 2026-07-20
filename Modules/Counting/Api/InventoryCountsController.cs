using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using verii_metivon_api.Modules.Counting.Application.Services;
namespace verii_metivon_api.Modules.Counting.Api;
[ApiController,Authorize,Route("api/inventory-counts")]public sealed class InventoryCountsController(IInventoryCountService s):ControllerBase{
[HttpGet]public async Task<IActionResult>List([FromQuery]InventoryCountQuery q,CancellationToken ct){var r=await s.GetPagedAsync(q,ct);return StatusCode(r.StatusCode,r);}
[HttpGet("{id:long}")]public async Task<IActionResult>Get(long id,CancellationToken ct){var r=await s.GetAsync(id,ct);return StatusCode(r.StatusCode,r);}
[HttpPost]public async Task<IActionResult>Create(CreateInventoryCountRequest x,CancellationToken ct){var r=await s.CreateAsync(x,ct);return StatusCode(r.StatusCode,r);}
[HttpPut("{id:long}")]public async Task<IActionResult>Update(long id,CreateInventoryCountRequest x,CancellationToken ct){var r=await s.UpdateAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
[HttpDelete("{id:long}")]public async Task<IActionResult>Delete(long id,CancellationToken ct){var r=await s.DeleteAsync(id,ct);return StatusCode(r.StatusCode,r);}
[HttpPost("{id:long}/entries")]public async Task<IActionResult>Enter(long id,IReadOnlyList<CountEntry>x,CancellationToken ct){var r=await s.EnterAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
[HttpPost("{id:long}/approve"),Authorize(Roles="Admin")]public async Task<IActionResult>Approve(long id,CancellationToken ct){var r=await s.ApproveAsync(id,ct);return StatusCode(r.StatusCode,r);}
[HttpPost("{id:long}/post")]public async Task<IActionResult>Post(long id,CancellationToken ct){var r=await s.PostAsync(id,ct);return StatusCode(r.StatusCode,r);}}
