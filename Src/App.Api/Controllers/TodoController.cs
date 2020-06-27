using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api.Models;
using App.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Todo>> Get()
        {
            await _todoRepository.InsertAsync(new Todo
            {
                Name = "Todo 1",
                Description = "",
                Status = 1,
                DueDate = DateTime.Now,
                DateCreated = DateTime.Now,
            });
            var todos = await _todoRepository.GetAllAsync();
            return todos;
        }
    }
}
