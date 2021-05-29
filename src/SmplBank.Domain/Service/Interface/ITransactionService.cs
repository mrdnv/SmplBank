using SmplBank.Domain.Dto.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service.Interface
{
    public interface ITransactionService
    {
        Task DepositAsync(int accountId, DepositTransactionDto dto);
        Task WithdrawAsync(int accountId, WithdrawalTransactionDto dto);
        Task TransferAsync(int accountId, TransferTransactionDto dto);
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(int accountId);
    }
}
