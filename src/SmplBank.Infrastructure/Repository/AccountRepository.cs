using Dapper;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Repository;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SmplBank.Infrastructure.Repository
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public Task<bool> DoesAccountNumberExistAsync(string accountNumber)
        {
            const string query = "SELECT 1 FROM [dbo].[Account] WHERE [AccountNumber] = @accountNumber";

            return this.dbConnection.ExecuteScalarAsync<bool>(query, new { accountNumber });
        }

        public Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            const string query = "SELECT * FROM [dbo].[Account] WHERE [AccountNumber] = @accountNumber";

            return this.dbConnection.QuerySingleOrDefaultAsync<Account>(query, new { accountNumber });
        }

        public Task<IEnumerable<Account>> GetByUserId(int userId)
        {
            const string query = "SELECT * FROM [dbo].[Account] WHERE [UserId] = @userId";

            return this.dbConnection.QueryAsync<Account>(query, new { userId });
        }
    }
}
