using App.Api.Models;
using Microsoft.Extensions.Configuration;

namespace App.Api.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(string tableName, IConfiguration config) : base(tableName, config)
        {
        }
    }
}
