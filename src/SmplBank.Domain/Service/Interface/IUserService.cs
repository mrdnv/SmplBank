using SmplBank.Domain.Dto.User;
using SmplBank.Domain.Entity;
using System.Threading.Tasks;

namespace SmplBank.Domain.Service.Interface
{
    public interface IUserService
    {
        Task<User> GetByCredentialAsync(string username, string password);
        Task InsertUserAsync(InsertUserDto dto);
    }
}
