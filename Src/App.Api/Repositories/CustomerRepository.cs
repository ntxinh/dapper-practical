using App.Api.Models;
using Microsoft.Extensions.Configuration;

namespace App.Api.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        // #1
        // public CustomerRepository(string tableName, IConfiguration config) : base(tableName, config)
        // {
        // }

        // #2
        public CustomerRepository(IConfiguration config) : base(config)
        {
        }
        public override string _tableName { get; } = "Customers";
    }
}
