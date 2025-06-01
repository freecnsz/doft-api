using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Reminder;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Reminder
{
    public class GetRemindersByTaskIdCommand : IRequest<ApiResponse<List<GetRemindersByTaskIdResponseDto>>>
    {
        public int TaskId { get; set; }

        public GetRemindersByTaskIdCommand(int taskId)
        {
            TaskId = taskId;
        }
    }

    public class GetRemindersByTaskIdCommandHandler : IRequestHandler<GetRemindersByTaskIdCommand, ApiResponse<List<GetRemindersByTaskIdResponseDto>>>
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ILogger<GetRemindersByTaskIdCommandHandler> _logger;

        public GetRemindersByTaskIdCommandHandler(IReminderRepository reminderRepository, ILogger<GetRemindersByTaskIdCommandHandler> logger)
        {
            _logger = logger;
            _reminderRepository = reminderRepository;
        }

        public async Task<ApiResponse<List<GetRemindersByTaskIdResponseDto>>> Handle(GetRemindersByTaskIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reminders = await _reminderRepository.GetReminderByTaskIdAsync(request.TaskId);
                if (reminders == null)
                {
                    _logger.LogError($"No reminders found for task {request.TaskId}.");
                    return ApiResponse<List<GetRemindersByTaskIdResponseDto>>.Error(404, "No reminders found for this task.", null);
                }

                var response = reminders.Select(r => new GetRemindersByTaskIdResponseDto
                {
                    Id = r.Id,
                    TaskId = r.TaskId,
                    TaskTitle = r.DoftTask.Detail.Title,
                    TaskDescription = r.DoftTask.Detail.Description,
                    TaskStatus = r.DoftTask.Status.ToString(),
                    TaskPriority = r.DoftTask.Priority.ToString(),
                    TaskDueDate = r.DoftTask.DueDate,
                    ReminderTime = r.ReminderTime,
                    ReminderPeriod = r.ReminderPeriod,
                    CreatedAt = r.CreatedAt
                }).ToList();

                _logger.LogInformation("Successfully retrieved reminders for task {TaskId}", request.TaskId);
                return ApiResponse<List<GetRemindersByTaskIdResponseDto>>.Success(200, "Reminders retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reminders for task {TaskId}", request.TaskId);
                return ApiResponse<List<GetRemindersByTaskIdResponseDto>>.Error(500, "An error occurred while retrieving reminders.", null);
            }
        }
    }
   
}