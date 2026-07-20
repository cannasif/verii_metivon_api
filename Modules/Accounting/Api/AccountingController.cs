using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using verii_metivon_api.Modules.Accounting.Application.Services;

namespace verii_metivon_api.Modules.Accounting.Api;

[ApiController,Authorize,Route("api/accounting")]
public sealed class AccountingController(IAccountingService service):ControllerBase
{
    [HttpGet("accounts")]public async Task<IActionResult>Accounts([FromQuery]LedgerAccountQuery q,CancellationToken ct){var r=await service.GetAccountsAsync(q,ct);return StatusCode(r.StatusCode,r);}
    [HttpGet("accounts/{id:long}")]public async Task<IActionResult>Account(long id,CancellationToken ct){var r=await service.GetAccountAsync(id,ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("accounts")]public async Task<IActionResult>CreateAccount(SaveLedgerAccountRequest x,CancellationToken ct){var r=await service.CreateAccountAsync(x,ct);return StatusCode(r.StatusCode,r);}
    [HttpPut("accounts/{id:long}")]public async Task<IActionResult>UpdateAccount(long id,SaveLedgerAccountRequest x,CancellationToken ct){var r=await service.UpdateAccountAsync(id,x,ct);return StatusCode(r.StatusCode,r);}
    [HttpDelete("accounts/{id:long}")]public async Task<IActionResult>DeleteAccount(long id,CancellationToken ct){var r=await service.DeleteAccountAsync(id,ct);return StatusCode(r.StatusCode,r);}
    [HttpGet("journals")]public async Task<IActionResult>Journals([FromQuery]JournalQuery q,CancellationToken ct){var r=await service.GetJournalsAsync(q,ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("journals")]public async Task<IActionResult>Journal(CreateJournalRequest x,CancellationToken ct){var r=await service.CreateJournalAsync(x,ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("journals/{id:long}/post")]public async Task<IActionResult>Post(long id,CancellationToken ct){var r=await service.PostJournalAsync(id,ct);return StatusCode(r.StatusCode,r);}
}
