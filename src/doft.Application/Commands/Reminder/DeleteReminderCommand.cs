using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Reminder
{
    public class DeleteReminderCommand : IRequest<ApiResponse<bool>>
    {
        public int ReminderId { get; set; }

        public DeleteReminderCommand(int reminderId)
        {
            ReminderId = reminderId;
        }
    }

    public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand, ApiResponse<bool>>
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ILogger<DeleteReminderCommandHandler> _logger;

        public DeleteReminderCommandHandler(IReminderRepository reminderRepository, ILogger<DeleteReminderCommandHandler> logger)
        {
            _reminderRepository = reminderRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var reminder = await _reminderRepository.GetByIdAsync(request.ReminderId);
                if (reminder == null)
                {
                    _logger.LogError($"Reminder with ID {request.ReminderId} not found.");
                    return ApiResponse<bool>.Error(404, "Reminder not found", false);
                }

                await _reminderRepository.DeleteAsync(reminder);

                return ApiResponse<bool>.Success(200, "Reminder deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting reminder with ID {request.ReminderId}");
                return ApiResponse<bool>.Error(500, "Internal server error", false);
            }
        }
    }
   
}