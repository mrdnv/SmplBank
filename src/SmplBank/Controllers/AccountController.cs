using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests;
using SmplBank.Domain.Entity;
using SmplBank.Filters;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    public class AccountController : AuthorizedController
    {
        public AccountController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAccountInfoAsync()
        {
            var request = CreateAuthorizedRequest<GetAccountInfoRequest>();
            var account = await this.mediator.Send(request);

            return Ok(account);
        }

        [HttpPost("deposit")]
        [Transaction]
        public async Task<IActionResult> DepositAsync([FromBody] DepositTransactionRequest request)
        {
            await this.mediator.Send(request);

            return Ok();
        }

        [HttpPost("withdraw")]
        [Transaction]
        public async Task<IActionResult> WithdrawAsync([FromBody] WithdrawalTransactionRequest request)
        {
            await this.mediator.Send(request);

            return Ok();
        }

        [HttpPost("transfer")]
        [Transaction]
        public async Task<IActionResult> TransferAsync([FromBody] TransferTransactionRequest request)
        {
            await this.mediator.Send(request);

            return Ok();
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactionsAsync()
        {
            var request = CreateAuthorizedRequest<GetAllTransactionRequest>();
            var transactions = await this.mediator.Send(request);

            return Ok(transactions);
        }
    }
}
