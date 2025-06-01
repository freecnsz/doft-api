using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Reminder
{
    public class AddReminderCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime ReminderTime { get; set; }
        public ReminderPeriod ReminderPeriod { get; set; }

        public AddReminderCommand(string userId, int taskId, DateTime reminderTime, ReminderPeriod reminderPeriod)
        {
            UserId = userId;
            TaskId = taskId;
            ReminderTime = reminderTime;
            ReminderPeriod = reminderPeriod;
        }
    }

    public class AddReminderCommandHandler : IRequestHandler<AddReminderCommand, ApiResponse<bool>>
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<AddReminderCommandHandler> _logger;

        public AddReminderCommandHandler(IReminderRepository reminderRepository, ITaskRepository taskRepository, ILogger<AddReminderCommandHandler> logger)
        {
            _reminderRepository = reminderRepository;
            _taskRepository = taskRepository;
            _logger = logger;
        }
        public async Task<ApiResponse<bool>> Handle(AddReminderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.TaskId);
                if (task == null)
                {
                    _logger.LogError($"Task with ID {request.TaskId} not found.");
                    return ApiResponse<bool>.Error(404, "Task not found", false);
                }

                var reminder = new Domain.Entities.Reminder
                {
                    TaskId = request.TaskId,
                    UserId = request.UserId,
                    ReminderTime = request.ReminderTime,
                    ReminderPeriod = request.ReminderPeriod,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _reminderRepository.AddAsync(reminder);
                if (result == null)
                {
                    _logger.LogError("Failed to add reminder.");
                    return ApiResponse<bool>.Error(500, "Failed to add reminder", false);
                }

                _logger.LogInformation($"Reminder added successfully for Task ID {request.TaskId}.");
                return ApiResponse<bool>.Success(200, "Reminder added successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding the reminder: {ex.Message}");
                return ApiResponse<bool>.Error(500, "An error occurred while adding the reminder", false);
            }
        }
    }

}