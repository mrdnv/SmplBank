using System.Threading.Tasks;

namespace SmplBank.Domain.Service.Interface
{
    public interface IBackgroundService
    {
        Task ExecuteAsync();
    }
}
