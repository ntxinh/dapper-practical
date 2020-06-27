using AutoMapper;
using App.Application.Tasks.Commands;
using App.Application.Tasks.Dto;
using App.Core.Entities;

namespace App.Application.Tasks.MappingProfiles
{
    public class TaskMappingProfile: Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<CreateTaskCommand, Task>();
            CreateMap<UpdateTaskCommand, Task>();
            CreateMap<Task, TaskDto>();
        }
    }
}
