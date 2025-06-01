using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Event
{
    public class DeleteEventCommand(int EventId) : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int EventId { get; set; } = EventId;
    }

    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, ApiResponse<bool>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<DeleteEventCommandHandler> _logger;

        public DeleteEventCommandHandler(IEventRepository eventRepository, ILogger<DeleteEventCommandHandler> logger)
        {
            _logger = logger;
            _eventRepository = eventRepository;
        }
      

        public async Task<ApiResponse<bool>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var eventToDelete = await _eventRepository.GetByIdAsync(request.EventId);
                if (eventToDelete == null)
                {
                    _logger.LogWarning("Event with ID {EventId} not found", request.EventId);
                    return ApiResponse<bool>.Error(404, "Event not found", false);
                }

                eventToDelete.Detail.IsDeleted = true;
                await _eventRepository.UpdateAsync(eventToDelete);

                _logger.LogInformation("Event with ID {EventId} deleted successfully", request.EventId);
                return ApiResponse<bool>.Success(200, "Event deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event with ID {EventId}", request.EventId);
                return ApiResponse<bool>.Error(500, "An error occurred while deleting the event", false);
            }
        }
    }
    
}