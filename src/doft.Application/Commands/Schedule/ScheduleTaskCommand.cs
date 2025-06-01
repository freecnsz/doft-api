using System.Text.Json.Serialization;
using doft.Application.DTOs;
using doft.Application.DTOs.Schedule;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Schedule
{
    public class ScheduleTaskCommand : IRequest<ApiResponse<ScheduledTaskResultDto>>
    {
        public int TaskId { get; set; }
        [JsonIgnore]
        public string OwnerId { get; set; }
        public Consequence Consequence { get; set; }
        public DueDateOption DueDateOption { get; set; }
        public Duration Duration { get; set; }
        public Urgency Urgency { get; set; }

        public ScheduleTaskCommand(
            int taskId,
            Consequence consequence,
            DueDateOption dueDateOption,
            Duration duration,
            Urgency urgency)
        {
            TaskId = taskId;
            Consequence = consequence;
            DueDateOption = dueDateOption;
            Duration = duration;
            Urgency = urgency;
        }
    }

    public class ScheduleTaskCommandHandler : IRequestHandler<ScheduleTaskCommand, ApiResponse<ScheduledTaskResultDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskSchedulingService _schedulingService;
        private readonly ILogger<ScheduleTaskCommandHandler> _logger;

        public ScheduleTaskCommandHandler(
            ITaskRepository taskRepository,
            ITaskSchedulingService schedulingService,
            ILogger<ScheduleTaskCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _schedulingService = schedulingService;
            _logger = logger;
        }

        public async Task<ApiResponse<ScheduledTaskResultDto>> Handle(ScheduleTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId);
                if (task == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found", request.TaskId);
                    return ApiResponse<ScheduledTaskResultDto>.NotFound("Task not found");
                }

                if (task.OwnerId != request.OwnerId)
                {
                    _logger.LogWarning("Unauthorized attempt to schedule task {TaskId} by user {UserId}", request.TaskId, request.OwnerId);
                    return ApiResponse<ScheduledTaskResultDto>.Error(403, "Unauthorized access to task", null);
                }

                var scheduleResult = await _schedulingService.ScheduleSingleTaskAsync(
                    task,
                    request.Consequence,
                    request.DueDateOption,
                    request.Duration,
                    request.Urgency,
                    cancellationToken);

                if (!scheduleResult.Success)
                {
                    _logger.LogWarning("Failed to schedule task {TaskId}: {Message}", request.TaskId, scheduleResult.Message);
                    return ApiResponse<ScheduledTaskResultDto>.Error(400, scheduleResult.Message, null);
                }

                _logger.LogInformation("Task {TaskId} scheduled successfully", request.TaskId);
                return ApiResponse<ScheduledTaskResultDto>.Success(200, "Task scheduled successfully", scheduleResult.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling task {TaskId}", request.TaskId);
                return ApiResponse<ScheduledTaskResultDto>.Error(500, "An error occurred while scheduling the task", null);
            }
        }
    }
} 