using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using src.doft.Application.DTOs.Task;

namespace src.doft.Application.Commands.Task
{
    public class GetTaskStatisticsCommand : IRequest<ApiResponse<GetTaskStatisticsResponseDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
    }

    public class GetTaskStatisticsCommandHandler : IRequestHandler<GetTaskStatisticsCommand, ApiResponse<GetTaskStatisticsResponseDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<GetTaskStatisticsCommandHandler> _logger;

        public GetTaskStatisticsCommandHandler(ITaskRepository taskRepository, ILogger<GetTaskStatisticsCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }


        public async Task<ApiResponse<GetTaskStatisticsResponseDto>> Handle(GetTaskStatisticsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var completedTasks = await _taskRepository.GetCompletedTasksForUser(request.UserId, cancellationToken);
                var remainingTasks = await _taskRepository.GetRemainingTasksForUser(request.UserId, cancellationToken);

                _logger.LogInformation("Retrieved task statistics for user {UserId}: CompletedTasks={CompletedTasks}, RemainingTasks={RemainingTasks}",
                    request.UserId, completedTasks.Count, remainingTasks.Count);

                return ApiResponse<GetTaskStatisticsResponseDto>.Success(200, "Task statistics retrieved successfully", new GetTaskStatisticsResponseDto
                {
                    CompletedTasks = completedTasks.Count,
                    RemainingTasks = remainingTasks.Count
                });
            }
            catch (System.Exception)
            {
                _logger.LogError("An error occurred while retrieving task statistics for user {UserId}", request.UserId);
                return ApiResponse<GetTaskStatisticsResponseDto>.Error(500, "An error occurred while retrieving task statistics", null);
            }
        }
    }
}