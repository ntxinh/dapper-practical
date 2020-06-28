using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using App.Api.Entities;

namespace App.Api.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntityAudit
    {
        // #2
        string _tableName { get; }

        // Basic operations
        Task<T> FirstOrDefaultByIdAsync(int id);
        Task<T> SingleOrDefaultByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> InsertAsync(T entity);
        Task<int> UpdateByIdAsync(T entity);
        Task<int> DeleteByIdAsync(int id);

        // Advanced operations
        Task<int> DeleteSoftByIdAsync(int id);
        Task<IEnumerable<T>> GetAllSoftDeletedAsync();
        Task<int> InsertRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<int> ExecuteSpAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}
