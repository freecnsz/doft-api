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
    public class GetRemindersByUserIdCommand : IRequest<ApiResponse<List<GetRemindersByTaskIdResponseDto>>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetRemindersByUserIdCommand(string userId)
        {
            UserId = userId;
        }
    }

    public class GetRemindersByUserIdCommandHandler : IRequestHandler<GetRemindersByUserIdCommand, ApiResponse<List<GetRemindersByTaskIdResponseDto>>>
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ILogger<GetRemindersByUserIdCommandHandler> _logger;

        public GetRemindersByUserIdCommandHandler(IReminderRepository reminderRepository, ILogger<GetRemindersByUserIdCommandHandler> logger)
        {
            _logger = logger;
            _reminderRepository = reminderRepository;
        }

        public async Task<ApiResponse<List<GetRemindersByTaskIdResponseDto>>> Handle(GetRemindersByUserIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reminders = await _reminderRepository.GetRemindersByUserIdAsync(request.UserId);
                if (reminders == null)
                {
                    _logger.LogError($"No reminders found for user {request.UserId}.");
                    return ApiResponse<List<GetRemindersByTaskIdResponseDto>>.Error(404, "No reminders found for this user.", null);
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

                _logger.LogInformation("Successfully retrieved reminders for user {UserId}", request.UserId);
                return ApiResponse<List<GetRemindersByTaskIdResponseDto>>.Success(200, "Reminders retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reminders for user {UserId}", request.UserId);
                return ApiResponse<List<GetRemindersByTaskIdResponseDto>>.Error(500, "An error occurred while retrieving reminders.", null);
            }
        }
    }
   
}