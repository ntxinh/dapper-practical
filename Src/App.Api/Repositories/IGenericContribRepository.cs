using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using App.Api.Entities;

namespace App.Api.Repositories
{
    public interface IGenericContribRepository<TEntity> where TEntity : BaseEntityAudit
    {
        // #2
        string _tableName { get; }

        // Basic operations
        Task<TEntity> GetAsync(int id);
        Task<TEntity> GetAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAsync(params TEntity[] entities);
        Task<IEnumerable<TEntity>> GetAsync(params int[] ids);
        Task<IEnumerable<TEntity>> GetAsync(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> GetAsync(IEnumerable<int> ids);
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<int> AddAsync(TEntity entity);
        Task<int> AddAsync(params TEntity[] entities);
        Task<int> AddAsync(IEnumerable<TEntity> entities);

        Task<int> DeleteAsync(int id);
        Task<bool> DeleteAsync(TEntity entity);
        Task<int> DeleteAsync(params TEntity[] entities);
        Task<int> DeleteAsync(params int[] ids);
        Task<int> DeleteAsync(IEnumerable<TEntity> entities);
        Task<int> DeleteAsync(IEnumerable<int> ids);
        Task<bool> DeleteAllAsync();

        Task<bool> UpdateAsync(TEntity entity);
        Task<int> UpdateAsync(params TEntity[] entities);
        Task<int> UpdateAsync(IEnumerable<TEntity> entities);

        // Advanced operations
        Task<int> DeleteSoftAsync(int id);
        Task<int> DeleteSoftAsync(TEntity entity);
        Task<int> DeleteSoftAsync(params TEntity[] entities);
        Task<int> DeleteSoftAsync(params int[] ids);
        Task<int> DeleteSoftAsync(IEnumerable<TEntity> entities);
        Task<int> DeleteSoftAsync(IEnumerable<int> ids);
        Task<int> DeleteSoftAllAsync();

        Task<IEnumerable<TEntity>> GetAllSoftDeletedAsync();

        Task<IEnumerable<TProjection>> QueryAsync<TProjection>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<int> ExecuteSpAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}
