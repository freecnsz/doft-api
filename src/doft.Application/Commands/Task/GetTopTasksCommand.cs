using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Task;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Task
{
    public class GetTopTasksCommand : IRequest<ApiResponse<List<GetTopTasksResponseDto>>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetTopTasksCommand(string userId)
        {
            UserId = userId;
        }

    }
    
        public class GetTopTasksCommandHandler : IRequestHandler<GetTopTasksCommand, ApiResponse<List<GetTopTasksResponseDto>>>
    {
        private readonly IPlannedTaskRepository _taskRepository;
        private readonly ILogger<GetTopTasksCommandHandler> _logger;

        public GetTopTasksCommandHandler(IPlannedTaskRepository taskRepository, ILogger<GetTopTasksCommandHandler> logger)
        {
            _logger = logger;
            _taskRepository = taskRepository;
        }

        public async Task<ApiResponse<List<GetTopTasksResponseDto>>> Handle(GetTopTasksCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tasks = await _taskRepository.GetTopTasksAsync(request.UserId);

                if (tasks == null)
                {
                    _logger.LogInformation("No top tasks found");
                    return ApiResponse<List<GetTopTasksResponseDto>>.NotFound("No top tasks found");
                }

                var response = tasks.Select(task => new GetTopTasksResponseDto(
                    task.Id,
                    task.DoftTask.Detail.Title,
                    task.DueDate.ToString("yyyy-MM-dd HH:mm:ss")
                )).ToList();
                return ApiResponse<List<GetTopTasksResponseDto>>.Success(200, "Top tasks retrieved successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top tasks");
                return ApiResponse<List<GetTopTasksResponseDto>>.Error(500, "An error occurred while retrieving top tasks", null);
            }
            
        }
    }
}