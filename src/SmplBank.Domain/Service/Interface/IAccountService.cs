using SmplBank.Domain.Dto.AccountDto;
using SmplBank.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service.Interface
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetByUserIdAsync(int userId);
        Task<AccountDto> FindAsync(int id);
    }
}
