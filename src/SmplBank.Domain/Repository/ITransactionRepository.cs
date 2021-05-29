using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmplBank.Domain.Repository
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetPendingDepositTransactions(int itemCount = 10);
        Task<IEnumerable<TransactionDto>> GetTransactionsByAccountId(int accountId);
    }
}
