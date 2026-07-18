using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using verii_metivon_api.Modules.Warehouses.Application.Services;
namespace verii_metivon_api.Modules.Warehouses.Api;
[ApiController,Authorize,Route("api/warehouses")]public sealed class WarehousesController(IWarehouseService s):ControllerBase{
 [HttpGet]public async Task<IActionResult>List([FromQuery]WarehouseListQuery q,CancellationToken ct){var r=await s.GetWarehousesAsync(q,ct);return StatusCode(r.StatusCode,r);}
 [HttpGet("locations")]public async Task<IActionResult>Locations([FromQuery]LocationListQuery q,CancellationToken ct){var r=await s.GetLocationsAsync(q,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost]public async Task<IActionResult>Create(SaveWarehouseRequest x,CancellationToken ct){var r=await s.CreateWarehouseAsync(x,ct);return StatusCode(r.StatusCode,r);}
 [HttpPost("locations")]public async Task<IActionResult>CreateLocation(SaveLocationRequest x,CancellationToken ct){var r=await s.CreateLocationAsync(x,ct);return StatusCode(r.StatusCode,r);}}
