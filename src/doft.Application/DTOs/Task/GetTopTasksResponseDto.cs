using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Commands.Task;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;

namespace doft.Application.DTOs.Task
{
    public class GetTopTasksResponseDto : IRequest<ApiResponse<List<GetTopTasksResponseDto>>>
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string DueDate { get; set; }

        public GetTopTasksResponseDto(int taskId, string title, string dueDate)
        {
            TaskId = taskId;
            Title = title;
            DueDate = dueDate;
        }
    }

}