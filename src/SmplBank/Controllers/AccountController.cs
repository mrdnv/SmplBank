using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests;
using SmplBank.Domain.Dto.AccountDto;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Filters;
using System.Collections.Generic;
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
            var account = await this.SendAuthorizedAsync<GetAccountInfoRequest, AccountDto>();

            return Ok(account);
        }

        [HttpPost("deposit")]
        [Transaction]
        public async Task<IActionResult> DepositAsync([FromBody] DepositTransactionRequest request)
        {
            await this.SendAsync(request);

            return Ok();
        }

        [HttpPost("withdraw")]
        [Transaction]
        public async Task<IActionResult> WithdrawAsync([FromBody] WithdrawalTransactionRequest request)
        {
            await this.SendAsync(request);

            return Ok();
        }

        [HttpPost("transfer")]
        [Transaction]
        public async Task<IActionResult> TransferAsync([FromBody] TransferTransactionRequest request)
        {
            await this.SendAsync(request);

            return Ok();
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactionsAsync()
        {
            var transactions = await this.SendAuthorizedAsync<GetAllTransactionRequest, IEnumerable<TransactionDto>>();

            return Ok(transactions);
        }
    }
}
