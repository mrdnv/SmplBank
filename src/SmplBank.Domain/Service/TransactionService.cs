using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Entity.Enum;
using SmplBank.Domain.Entity.Enums;
using SmplBank.Domain.Exception;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service.Interface;
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

        public TransactionService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
        }

        public async Task DepositAsync(int accountId, DepositTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ValidationDomainException("Amount must be a positive number.");

            var account = await this.accountRepository.FindAsync(accountId);

            if (account == null)
                throw new EntityNotFoundDomainException($"Account does not exist.");

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
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ValidationDomainException("Amount must be a positive number.");

            var fromAccount = await this.accountRepository.FindAsync(accountId);

            if (fromAccount == null)
                throw new EntityNotFoundDomainException($"From Account does not exist.");

            if (fromAccount.Balance < dto.Amount)
                throw new ValidationDomainException($"Insufficient balance.");

            var toAccount = await this.accountRepository.GetByAccountNumberAsync(dto.ToAccountNumber);

            if (toAccount == null)
                throw new EntityNotFoundDomainException($"To Account does not exist.");

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
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ValidationDomainException("Amount must be a positive number.");

            var account = await this.accountRepository.FindAsync(accountId);

            if (account == null)
                throw new EntityNotFoundDomainException($"Account does not exist.");

            if (account.Balance < dto.Amount)
                throw new ValidationDomainException($"Insufficient balance.");

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
