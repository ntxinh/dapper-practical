using App.Api.Models;
using Microsoft.Extensions.Configuration;

namespace App.Api.Repositories
{
    public class TodoRepository : GenericRepository<Todo>, ITodoRepository
    {
        public TodoRepository(IConfiguration config) : base(config)
        {
        }
        public override string _tableName { get; } = "Todos";
    }
}
