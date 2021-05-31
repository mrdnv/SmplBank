using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Entity.Enum;
using SmplBank.Domain.Entity.Enums;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service.Interface;
using SmplBank.Domain.Validation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IValidatorContainer<Transaction> transactionValidatorFactory;

        public TransactionService(IAccountRepository accountRepository, 
            ITransactionRepository transactionRepository, 
            IValidatorContainer<Transaction> transactionValidatorFactory)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.transactionValidatorFactory = transactionValidatorFactory;
        }

        public async Task<int> DepositAsync(DepositTransactionDto dto)
        {
            var account = (await this.transactionValidatorFactory
                .GetValidator<DepositTransactionDto>()
                .ValidateAsync(dto))
                .GetFederatedObject<Account>(_ => _.AccountId);

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = dto.Amount,
                Type = TransactionType.Deposit,
                Status = TransactionStatus.Pending,
                Description = $"Deposit ${dto.Amount}."
            };

            return await this.transactionRepository.InsertAsync(transaction);
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(int accountId)
            => (await this.transactionRepository.GetTransactionsByAccountId(accountId))
                .OrderByDescending(_ => _.TransactionDate);

        public async Task TransferAsync(TransferTransactionDto dto)
        {
            var validatedObject = await this.transactionValidatorFactory
                .GetValidator<TransferTransactionDto>()
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

        public async Task<int> WithdrawAsync(WithdrawalTransactionDto dto)
        {
            var account = (await this.transactionValidatorFactory
                .GetValidator<WithdrawalTransactionDto>()
                .ValidateAsync(dto))
                .GetFederatedObject<Account>(_ => _.AccountId);

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = dto.Amount,
                Type = TransactionType.Withdraw,
                Status = TransactionStatus.Pending,
                Description = $"Withdraw ${dto.Amount}.",
                EndBalance = null
            };

            return await this.transactionRepository.InsertAsync(transaction);
        }


        public async Task ProcessTransaction(int transactionId)
        {
            var transaction = await this.transactionRepository.FindAsync(transactionId);

            if (transaction.Status != Entity.Enum.TransactionStatus.Pending)
                return;

            var amount = transaction.Amount;

            switch (transaction.Type)
            {
                case Entity.Enums.TransactionType.Deposit:
                    break;
                case Entity.Enums.TransactionType.Withdraw:
                    amount *= -1;
                    break;
                default:
                    return;
            }

            var account = await this.accountRepository.FindAsync(transaction.AccountId);
            account.Balance += amount;
            await this.accountRepository.UpdateAsync(account);

            transaction.Status = Entity.Enum.TransactionStatus.Completed;
            transaction.EndBalance = account.Balance;
            await this.transactionRepository.UpdateAsync(transaction);
        }
    }
}
