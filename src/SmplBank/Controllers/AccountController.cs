using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Service.Interface;
using SmplBank.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmplBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ITransactionService transactionService;
        private readonly IAccountService accountService;

        public AccountController(ITransactionService transactionService, IAccountService accountService) : this()
        {
            this.transactionService = transactionService;
            this.accountService = accountService;
        }

        protected AccountController()
        {
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAccountInfoAsync()
        {
            var account = await accountService.FindAsync(AccountId);

            return Ok(account);
        }

        [HttpPost("deposit")]
        [Transaction]
        public async Task<IActionResult> DepositAsync([FromBody] DepositTransactionDto dto)
        {
            await this.transactionService.DepositAsync(AccountId, dto);

            return Ok();
        }

        [HttpPost("withdraw")]
        [Transaction]
        public async Task<IActionResult> WithdrawAsync([FromBody] WithdrawalTransactionDto dto)
        {
            await this.transactionService.WithdrawAsync(AccountId, dto);

            return Ok();
        }

        [HttpPost("transfer")]
        [Transaction]
        public async Task<IActionResult> TransferAsync([FromBody] TransferTransactionDto dto)
        {
            await this.transactionService.TransferAsync(AccountId, dto);

            return Ok();
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactionsAsync()
        {
            var transactions = await this.transactionService.GetAllTransactionsAsync(AccountId);

            return Ok(transactions);
        }

        private int AccountId => int.Parse(
            (HttpContext.User.Identity as ClaimsIdentity)
            .FindFirst($"{nameof(Account)}{nameof(Account.Id)}").Value);
    }
}
