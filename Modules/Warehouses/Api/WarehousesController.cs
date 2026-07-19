using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using verii_metivon_api.Modules.Warehouses.Application.Services;
namespace verii_metivon_api.Modules.Warehouses.Api;
[ApiController,Authorize,Route("api/warehouses")]public sealed class WarehousesController(IWarehouseService s):ControllerBase{
 [HttpGet]public async Task<IActionResult>List([FromQuery]WarehouseListQuery q,CancellationToken ct){var r=await s.GetWarehousesAsync(q,ct);return StatusCode(r.StatusCode,r);}
 [HttpGet("locations")]public async Task<IActionResult>Locations([FromQuery]LocationListQuery q,CancellationToken ct){var r=await s.GetLocationsAsync(q,ct);return StatusCode(r.StatusCode,r);}
 [HttpGet("{id:long}")]public async Task<IActionResult>Get(long id,CancellationToken ct){var r=await s.GetWarehouseAsync(id,ct);return StatusCode(r.StatusCode,r);}
 [HttpGet("locations/{id:long}")]public async Task<IActionResult>GetLocation(long id,CancellationToken ct){var r=await s.GetLocationAsync(id,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost]public async Task<IActionResult>Create(SaveWarehouseRequest x,CancellationToken ct){var r=await s.CreateWarehouseAsync(x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("locations")]public async Task<IActionResult>CreateLocation(SaveLocationRequest x,CancellationToken ct){var r=await s.CreateLocationAsync(x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("{id:long}/update")]public async Task<IActionResult>Update(long id,SaveWarehouseRequest x,CancellationToken ct){var r=await s.UpdateWarehouseAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("locations/{id:long}/update")]public async Task<IActionResult>UpdateLocation(long id,SaveLocationRequest x,CancellationToken ct){var r=await s.UpdateLocationAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("{id:long}/delete")]public async Task<IActionResult>Delete(long id,CancellationToken ct){var r=await s.DeleteWarehouseAsync(id,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("locations/{id:long}/delete")]public async Task<IActionResult>DeleteLocation(long id,CancellationToken ct){var r=await s.DeleteLocationAsync(id,ct);return StatusCode(r.StatusCode,r);}}
