using SmplBank.Domain.Entity;
using System.Threading.Tasks;

namespace SmplBank.Domain.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<bool> DoesUsernameExistAsync(string username);
    }
}
