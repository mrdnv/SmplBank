using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmplBank.Domain.Repository
{
    public interface IRepository<T> where T : Entity.Entity
    {
        Task<bool> CheckExist<TProperty>(Expression<Func<T, TProperty>> predicate, TProperty value);
        Task<bool> CheckExistAsync(int id);
        Task<T> FindAsync(int id);
        Task<int> InsertAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
