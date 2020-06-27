using MediatR;
using App.Application.Tasks.Dto;

namespace App.Application.Tasks.Queries
{
    public class GetTaskByIdQuery: IRequest<TaskDto>
    {
        public int Id { get; set; }
    }
}
