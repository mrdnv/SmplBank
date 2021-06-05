using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Application.Requests.Commands;
using SmplBank.Application.Requests.Queries;
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
        public AccountController(ISender sender) : base(sender)
        {
        }

        [HttpGet("")]
        public Task<IActionResult> GetAccountInfoAsync() => this.SendAuthorizedAsync<GetAccountInfoQuery, AccountDto>();

        [HttpPost("deposit")]
        [Transaction]
        public Task<IActionResult> DepositAsync([FromBody] DepositTransactionCommand request) => this.SendAsync(request);

        [HttpPost("withdraw")]
        [Transaction]
        public Task<IActionResult> WithdrawAsync([FromBody] WithdrawalTransactionCommand request) => this.SendAsync(request);

        [HttpPost("transfer")]
        [Transaction]
        public Task<IActionResult> TransferAsync([FromBody] TransferTransactionCommand request) => this.SendAsync(request);

        [HttpGet("transactions")]
        public Task<IActionResult> GetAllTransactionsAsync()
            => this.SendAuthorizedAsync<GetAllTransactionQuery, IEnumerable<TransactionDto>>();
    }
}
