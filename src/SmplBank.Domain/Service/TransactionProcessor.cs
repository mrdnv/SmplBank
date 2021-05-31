using SmplBank.Domain.Repository;
using SmplBank.Domain.Service.Interface;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SmplBank.Domain.Service
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly SemaphoreSlim semaphoreSlim;

        public TransactionProcessor(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task ExecuteAsync()
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                var transactions = await this.transactionRepository.GetPendingTransferReceiveTransactions();

                foreach (var transaction in transactions)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var account = await this.accountRepository.FindAsync(transaction.AccountId);
                        account.Balance += transaction.Amount;
                        await this.accountRepository.UpdateAsync(account);

                        transaction.Status = Entity.Enum.TransactionStatus.Completed;
                        transaction.EndBalance = account.Balance;
                        await this.transactionRepository.UpdateAsync(transaction);

                        scope.Complete();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
