using System;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Task;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Task
{
    public class GetTaskByIdCommand : IRequest<ApiResponse<GetTaskByIdResponseDto>>
    {
        public int TaskId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string UserId { get; set; }

        public GetTaskByIdCommand(int taskId, string userId)
        {
            TaskId = taskId;
            UserId = userId;
        }
    }

    public class GetTaskByIdCommandHandler : IRequestHandler<GetTaskByIdCommand, ApiResponse<GetTaskByIdResponseDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<GetTaskByIdCommandHandler> _logger;

        public GetTaskByIdCommandHandler(ITaskRepository taskRepository, ILogger<GetTaskByIdCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<GetTaskByIdResponseDto>> Handle(GetTaskByIdCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null ||task.OwnerId != request.UserId)
            {
                _logger.LogWarning("Task not found with ID: {TaskId}", request.TaskId);
                return ApiResponse<GetTaskByIdResponseDto>.NotFound("Task not found");
            }

            var response = new GetTaskByIdResponseDto
            {
                TaskId = task.Id,
                Title = task.Detail.Title,
                Description = task.Detail.Description,
                HasAttachment = task.Detail.HasAttachment,
                HasTag = task.Detail.HasTag,
                CreatedAt = task.Detail.CreatedAt,
                UpdatedAt = task.Detail.UpdatedAt,
                CategoryName = task.Category.Name,
                CategoryColor = task.Category.Color,
                DueDate = task.DueDate,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                PriorityScore = task.PriorityScore,
                RepeatId = task.RepeatId,
                Tags = task.TagLinks.Select(tl => tl.Tag.Name).ToList()
            };

            _logger.LogInformation("Successfully retrieved task {TaskId} for user {UserId}", request.TaskId, request.UserId);
            return ApiResponse<GetTaskByIdResponseDto>.Success(200, "Task retrieved successfully", response);
        }
    }
} 