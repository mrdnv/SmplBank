using SmplBank.Domain.Dto.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service.Interface
{
    public interface ITransactionService
    {
        Task<int> DepositAsync(DepositTransactionDto dto);
        Task<int> WithdrawAsync(WithdrawalTransactionDto dto);
        Task TransferAsync(TransferTransactionDto dto);
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(int accountId);
        Task ProcessTransaction(int transactionId);
    }
}
