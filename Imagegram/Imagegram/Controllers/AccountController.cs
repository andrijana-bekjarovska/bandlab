using System;
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
            try
            {
                var account = await _accountService.GetById(id);
                var response = new AccountResponse
                {
                    Id = account.Id,
                    Name = account.Name
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return ex.Message == ErrorCodes.ResourceNotFound ? NotFound() : StatusCode(500);
            }
        }


        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.Conflict)]
        public async Task<ActionResult<AccountResponse>> Post([FromBody] AccountRequest request)
        {
            try
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
            catch (Exception ex)
            {
                return ex.Message == ErrorCodes.AccountAlreadyExists ? Conflict() : StatusCode(500);
            }
        }

        [HttpDelete]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete([FromHeader(Name = "X-Account-Id")] string accountId)
        {
            try
            {
                await _accountService.DeleteAccount(accountId);
                return Ok();
            }
            catch (Exception ex)
            {
                return ex.Message == ErrorCodes.ResourceNotFound ? BadRequest() : StatusCode(500);
            }
        }
    }
}