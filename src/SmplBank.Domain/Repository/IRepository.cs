using System.Threading.Tasks;

namespace SmplBank.Domain.Repository
{
    public interface IRepository<T> where T : Entity.Entity
    {
        Task<T> FindAsync(int id);
        Task<int> InsertAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
