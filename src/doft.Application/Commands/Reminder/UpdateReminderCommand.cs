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
    public class UpdateReminderCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime ReminderTime { get; set; }
        public ReminderPeriod ReminderPeriod { get; set; }

        public UpdateReminderCommand(int id, string userId, int taskId, DateTime reminderTime, ReminderPeriod reminderPeriod)
        {
            Id = id;
            UserId = userId;
            TaskId = taskId;
            ReminderTime = reminderTime;
            ReminderPeriod = reminderPeriod;
        }

    }

    public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand, ApiResponse<bool>>
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ILogger<UpdateReminderCommandHandler> _logger;

        public UpdateReminderCommandHandler(IReminderRepository reminderRepository, ILogger<UpdateReminderCommandHandler> logger)
        {
            _reminderRepository = reminderRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reminder = await _reminderRepository.GetByIdAsync(request.Id);
                if (reminder == null)
                {
                    _logger.LogError($"Reminder with ID {request.Id} not found.");
                    return ApiResponse<bool>.Error(404, "Reminder not found", false);
                }

                reminder.TaskId = request.TaskId;
                reminder.ReminderTime = request.ReminderTime;
                reminder.ReminderPeriod = request.ReminderPeriod;

                await _reminderRepository.UpdateAsync(reminder);

                _logger.LogInformation($"Successfully updated reminder with ID {request.Id}.");
                return ApiResponse<bool>.Success(200, "Reminder updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the reminder.");
                return ApiResponse<bool>.Error(500, "An error occurred while updating the reminder", false);
            }
        }
    }



    
}