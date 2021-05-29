using Dapper;
using SmplBank.Domain.Dto.Transaction;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Entity.Enum;
using SmplBank.Domain.Entity.Enums;
using SmplBank.Domain.Repository;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SmplBank.Infrastructure.Repository
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private static int[] DepositTransactionTypes => new int[] { (int)TransactionType.Deposit, (int)TransactionType.TransferReceive };
        private static int TransferReceiveType => (int)TransactionType.TransferReceive;

        public TransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public Task<IEnumerable<Transaction>> GetPendingDepositTransactions(int itemCount = 10)
        {
            var query = $@"SELECT TOP {itemCount} *
                    FROM [dbo].[Transaction] 
                    WHERE Status = {(int)TransactionStatus.Pending}
                    AND Type IN @DepositTransactionTypes";

            return this.dbConnection.QueryAsync<Transaction>(query, new { DepositTransactionTypes });
        }

        public Task<IEnumerable<TransactionDto>> GetTransactionsByAccountId(int accountId)
        {
            var query = $@"
SELECT acc.[AccountNumber] as [FromAccountNumber]
, linkedAcc.[AccountNumber] as [ToAccountNumber]
, trs.[Type] as [TransactionType]
, trs.[Status] as [TransactionStatus]
, trs.[CreatedOn] as [TransactionDate]
, trs.[Amount]
, trs.[EndBalance]
, trs.[Description]
FROM [dbo].[Transaction] trs
INNER JOIN [dbo].[Account] acc ON trs.[AccountId] = acc.[Id]
LEFT JOIN [dbo].[Transaction] linkedTrs ON trs.[LinkedTransactionId] = linkedTrs.[Id]
LEFT JOIN [dbo].[Account] linkedAcc ON linkedTrs.[AccountId] = linkedAcc.[Id]
WHERE acc.[Id] = @accountId AND trs.[Type] <> @TransferReceiveType
UNION
(SELECT 
linkedAcc.[AccountNumber] as [FromAccountNumber]
, acc.[AccountNumber] as [ToAccountNumber]
, trs.[Type] as [TransactionType]
, trs.[Status] as [TransactionStatus]
, trs.[CreatedOn] as [TransactionDate]
, trs.[Amount]
, trs.[EndBalance]
, trs.[Description]
FROM [dbo].[Transaction] trs
INNER JOIN [dbo].[Account] acc ON trs.[AccountId] = acc.[Id]
INNER JOIN [dbo].[Transaction] linkedTrs ON trs.[LinkedTransactionId] = linkedTrs.[Id]
INNER JOIN [dbo].[Account] linkedAcc ON linkedTrs.[AccountId] = linkedAcc.[Id]
WHERE acc.[Id] = @accountId AND trs.[Type] = @TransferReceiveType)";

            return this.dbConnection.QueryAsync<TransactionDto>(query, new { accountId, TransferReceiveType });
        }
    }
}
