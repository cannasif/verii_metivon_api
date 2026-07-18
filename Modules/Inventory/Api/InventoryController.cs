using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using verii_metivon_api.Modules.Inventory.Application.Services;
namespace verii_metivon_api.Modules.Inventory.Api;
[ApiController,Authorize,Route("api/inventory")]public sealed class InventoryController(IInventoryService s):ControllerBase{
[HttpGet("transactions")]public async Task<IActionResult>Transactions([FromQuery]InventoryTransactionQuery q,CancellationToken ct){var r=await s.GetTransactionsAsync(q,ct);return StatusCode(r.StatusCode,r);}
[HttpGet("balances")]public async Task<IActionResult>Balances([FromQuery]InventoryBalanceQuery q,CancellationToken ct){var r=await s.GetBalancesAsync(q,ct);return StatusCode(r.StatusCode,r);}
[HttpPost("post")]public async Task<IActionResult>Post(PostInventoryRequest x,CancellationToken ct){var r=await s.PostAsync(x,ct);return StatusCode(r.StatusCode,r);}}
