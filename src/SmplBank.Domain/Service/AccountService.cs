using SmplBank.Domain.Dto.AccountDto;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<AccountDto> FindAsync(int id)
        {
            var account = await this.accountRepository.FindAsync(id);

            return new AccountDto
            {
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                RegisteredOn = account.CreatedOn
            };
        }

        public Task<IEnumerable<Account>> GetByUserIdAsync(int userId)
            => this.accountRepository.GetByUserId(userId);
    }
}
