using Dapper;
using SmplBank.Domain.Entity;
using SmplBank.Domain.Repository;
using System.Data;
using System.Threading.Tasks;

namespace SmplBank.Infrastructure.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public Task<bool> DoesUsernameExistAsync(string username)
        {
            var query = "SELECT 1 FROM [dbo].[User] WHERE [Username] = @username";

            return this.dbConnection.ExecuteScalarAsync<bool>(query, new { username });
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            var query = "SELECT * FROM [dbo].[User] WHERE [Username] = @username";

            return this.dbConnection.QuerySingleOrDefaultAsync<User>(query, new { username });
        }
    }
}
