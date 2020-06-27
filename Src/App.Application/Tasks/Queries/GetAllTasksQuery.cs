using MediatR;
using System.Collections.Generic;
using App.Application.Tasks.Dto;

namespace App.Application.Tasks.Queries
{
    public class GetAllTasksQuery: IRequest<List<TaskDto>>
    {
    }
}
