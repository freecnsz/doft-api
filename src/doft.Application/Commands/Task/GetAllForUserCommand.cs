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
    public class GetAllForUserCommand : IRequest<ApiResponse<List<GetAllForUserResponseDto>>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetAllForUserCommand(string userId)
        {
            UserId = userId;
        }
    }

    public class GetAllForUserCommandHandler : IRequestHandler<GetAllForUserCommand, ApiResponse<List<GetAllForUserResponseDto>>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<GetAllForUserCommandHandler> _logger;

        public GetAllForUserCommandHandler(ITaskRepository taskRepository, ILogger<GetAllForUserCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetAllForUserResponseDto>>> Handle(GetAllForUserCommand request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetAllTasksForUserAsync(request.UserId);

            if (tasks == null || !tasks.Any())
            {
                _logger.LogWarning("No tasks found for user {UserId}", request.UserId);
                return ApiResponse<List<GetAllForUserResponseDto>>.NotFound("No tasks found for the user.");
            }

            var response = tasks
                .OrderBy(t => t.DueDate)
                .Select(t => new GetAllForUserResponseDto
                {
                    TaskId = t.Id,
                    Title = t.Detail.Title,
                    Description = t.Detail.Description,
                    HasAttachment = t.Detail.HasAttachment,
                    HasTag = t.Detail.HasTag,
                    CreatedAt = t.Detail.CreatedAt,
                    UpdatedAt = t.Detail.UpdatedAt,
                    CategoryName = t.Category.Name,
                    CategoryColor = t.Category.Color,
                    DueDate = t.DueDate,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    PriorityScore = t.PriorityScore,
                    RepeatId = t.RepeatId,
                    Tags = t.TagLinks.Select(tl => tl.Tag.Name).ToList()
                }).ToList();

            _logger.LogInformation("Successfully retrieved {Count} tasks for user {UserId}", response.Count, request.UserId);
            return ApiResponse<List<GetAllForUserResponseDto>>.Success(200, "Tasks retrieved successfully.", response);
        }
    }
}