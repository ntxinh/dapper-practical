using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Api.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task DeleteRowAsync(Guid id);
        Task<T> GetAsync(Guid id);
        Task<int> SaveRangeAsync(IEnumerable<T> list);
        Task InsertAsync(T t);
        Task UpdateAsync(T t);
    }

}
