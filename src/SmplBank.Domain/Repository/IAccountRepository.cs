using SmplBank.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmplBank.Domain.Repository
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task<bool> DoesAccountNumberExistAsync(string accountNumber);
        Task<IEnumerable<Account>> GetByUserId(int userId);
    }
}
