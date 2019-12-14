using System.Net;
using System.Threading.Tasks;
using Imagegram.Models.Account;
using Imagegram.Services.Contracts;
using Imagegram.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<AccountResponse>> Get([FromRoute] string id)
        {
            var account = await _accountService.GetById(id);
            var response = new AccountResponse
            {
                Id = account.Id,
                Name = account.Name
            };
            return Ok(response);
        }


        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<AccountResponse>> Post([FromBody] AccountRequest request)
        {
            var account = new Account
            {
                Name = request.Name
            };

            var response = await _accountService.CreateAccount(account);
            return CreatedAtAction(nameof(Get), new {id = response.Id}, new AccountResponse
            {
                Id = response.Id,
                Name = response.Name
            });
        }

        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult> Delete([FromHeader(Name = "X-Account-Id")] string accountId)
        {
            await _accountService.DeleteAccount(accountId);
            return Ok();
        }
    }
}