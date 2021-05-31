using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Entity.Enum;
using SmplBank.Domain.Entity.Enums;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service.Interface;
using SmplBank.Domain.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IValidatorFactory<Transaction> transactionValidatorFactory;

        public TransactionService(IAccountRepository accountRepository, 
            ITransactionRepository transactionRepository, 
            IValidatorFactory<Transaction> transactionValidatorFactory)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.transactionValidatorFactory = transactionValidatorFactory;
        }

        public async Task DepositAsync(int accountId, DepositTransactionDto dto)
        {
            dto.AccountId = accountId;
            var account = (await this.transactionValidatorFactory
                .Resolve<DepositTransactionDto>()
                .ValidateAsync(dto))
                .GetFederatedObject<Account>(_ => _.AccountId);

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = dto.Amount,
                Type = TransactionType.Deposit,
                Description = $"Withdraw ${dto.Amount}."
            };

            await this.transactionRepository.InsertAsync(transaction);
            await this.accountRepository.UpdateAsync(account);
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(int accountId)
            => (await this.transactionRepository.GetTransactionsByAccountId(accountId))
                .OrderByDescending(_ => _.TransactionDate);

        public async Task TransferAsync(int accountId, TransferTransactionDto dto)
        {
            dto.FromAccountId = accountId;

            var validatedObject = await this.transactionValidatorFactory
                .Resolve<TransferTransactionDto>()
                .ValidateAsync(dto);

            var fromAccount = validatedObject.GetFederatedObject<Account>(_ => _.FromAccountId);
            var toAccount = validatedObject.GetFederatedObject<Account>(_ => _.ToAccountNumber);

            var withdrawalTransaction = new Transaction
            {
                AccountId = fromAccount.Id,
                Amount = dto.Amount,
                Type = TransactionType.Transfer,
                Status = TransactionStatus.Completed,
                Description = $"Transfer ${dto.Amount} to {toAccount.AccountNumber}."
            };

            var withdrawalTransactionId = await this.transactionRepository.InsertAsync(withdrawalTransaction);

            var depositTransaction = new Transaction
            {
                AccountId = toAccount.Id,
                Amount = dto.Amount,
                Type = TransactionType.TransferReceive,
                Description = $"Receive ${dto.Amount} from {fromAccount.AccountNumber}",
                LinkedTransactionId = withdrawalTransactionId
            };

            var depositTransactionId = await this.transactionRepository.InsertAsync(depositTransaction);

            fromAccount.Balance -= dto.Amount;
            await this.accountRepository.UpdateAsync(fromAccount);

            withdrawalTransaction = await this.transactionRepository.FindAsync(withdrawalTransactionId);
            withdrawalTransaction.LinkedTransactionId = depositTransactionId;
            withdrawalTransaction.EndBalance = fromAccount.Balance;
            await this.transactionRepository.UpdateAsync(withdrawalTransaction);
        }

        public async Task WithdrawAsync(int accountId, WithdrawalTransactionDto dto)
        {
            dto.AccountId = accountId;
            var account = (await this.transactionValidatorFactory
                .Resolve<WithdrawalTransactionDto>()
                .ValidateAsync(dto))
                .GetFederatedObject<Account>(_ => _.AccountId);

            account.Balance -= dto.Amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = dto.Amount,
                Type = TransactionType.Withdraw,
                Status = TransactionStatus.Completed,
                Description = $"Deposit ${dto.Amount}",
                EndBalance = account.Balance
            };

            await this.transactionRepository.InsertAsync(transaction);
            await this.accountRepository.UpdateAsync(account);
        }
    }
}
