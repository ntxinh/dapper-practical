using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Application.Tasks.Commands;
using App.Application.Tasks.Dto;
using App.Application.Tasks.Queries;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTaskCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll()
        {
            return await Mediator.Send(new GetAllTasksQuery());
        }

        [HttpGet("/{id}")]
        public async Task<ActionResult<TaskDto>> Get(int id)
        {
            return await Mediator.Send(new GetTaskByIdQuery { Id = id });
        }

        [HttpPut]
        public async Task<ActionResult<int>> Update(UpdateTaskCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpDelete]
        public async Task<ActionResult<int>> Delete(int id)
        {
            return await Mediator.Send(new DeleteTaskCommand { Id = id });
        }
    }
}
