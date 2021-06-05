using SmplBank.Domain.Entity;
using SmplBank.Domain.Repository;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SmplBank.Infrastructure.Exception;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SmplBank.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly IDbConnection dbConnection;

        public Repository(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public Task<bool> CheckExist<TProperty>(Expression<Func<T, TProperty>> predicate, TProperty value)
        {
            if (predicate.Body is not MemberExpression memberExpression)
                throw new ArgumentException("Predicate must be a property selector", nameof(predicate));

            var propertyName = memberExpression.Member.Name;
            var parameters = new DynamicParameters();
            parameters.Add(propertyName, value);

            var query = QueryHelper.GenerateCheckExistQuery<T>(propertyName);

            return this.dbConnection.ExecuteScalarAsync<bool>(query, parameters);
        }

        public Task<bool> CheckExistAsync(int id)
        {
            var query = QueryHelper.GenerateExistByIdQuery<T>();

            return this.dbConnection.ExecuteScalarAsync<bool>(query, new { Id = id });
        }

        public virtual Task<T> FindAsync(int id)
        {
            var query = QueryHelper.GenerateFindQuery<T>();

            return this.dbConnection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
        }

        public virtual Task<int> InsertAsync(T entity)
        {
            var query = QueryHelper.GenerateInsertQuery<T>();
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;

            return this.dbConnection.ExecuteScalarAsync<int>(query, entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var query = QueryHelper.GenerateUpdateQuery<T>();
            entity.UpdatedOn = DateTime.Now;
            var rowVersion = await this.dbConnection.ExecuteScalarAsync<byte[]>(query, entity);

            if (rowVersion == null)
                throw new ConcurrentException(typeof(T).Name);
        }
    }
}
