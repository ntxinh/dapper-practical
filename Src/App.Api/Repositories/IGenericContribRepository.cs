using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using App.Api.Entities;

namespace App.Api.Repositories
{
    public interface IGenericContribRepository<T> where T : BaseEntityAudit
    {
        // #2
        string _tableName { get; }

        // Basic operations
        Task<T> GetAsync(int id);
        Task<T> GetAsync(T entity);
        Task<IEnumerable<T>> GetAsync(params T[] entities);
        Task<IEnumerable<T>> GetAsync(params int[] ids);
        Task<IEnumerable<T>> GetAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAsync(IEnumerable<int> ids);
        Task<IEnumerable<T>> GetAllAsync();

        Task<int> AddAsync(T entity);
        Task<int> AddAsync(params T[] entities);
        Task<int> AddAsync(IEnumerable<T> entities);

        Task<int> DeleteAsync(int id);
        Task<bool> DeleteAsync(T entity);
        Task<int> DeleteAsync(params T[] entities);
        Task<int> DeleteAsync(params int[] ids);
        Task<int> DeleteAsync(IEnumerable<T> entities);
        Task<int> DeleteAsync(IEnumerable<int> ids);
        Task<bool> DeleteAllAsync();

        Task<bool> UpdateAsync(T entity);
        Task<int> UpdateAsync(params T[] entities);
        Task<int> UpdateAsync(IEnumerable<T> entities);

        // Advanced operations
        Task<int> DeleteSoftAsync(int id);
        Task<int> DeleteSoftAsync(T entity);
        Task<int> DeleteSoftAsync(params T[] entities);
        Task<int> DeleteSoftAsync(params int[] ids);
        Task<int> DeleteSoftAsync(IEnumerable<T> entities);
        Task<int> DeleteSoftAsync(IEnumerable<int> ids);
        Task<int> DeleteSoftAllAsync();

        Task<IEnumerable<T>> GetAllSoftDeletedAsync();

        Task<IEnumerable<T>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<int> ExecuteSpAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}
