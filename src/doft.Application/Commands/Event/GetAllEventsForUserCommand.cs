using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Event;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Event
{
    public class GetAllEventsForUserCommand(string userId) : IRequest<ApiResponse<List<GetAllEventsForUserResponseDto>>>
    {
        [JsonIgnore]
        public string UserId { get; set; } = userId;
    }

    public class GetAllEventsForUserCommandHandler : IRequestHandler<GetAllEventsForUserCommand, ApiResponse<List<GetAllEventsForUserResponseDto>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetAllEventsForUserCommandHandler> _logger;

        public GetAllEventsForUserCommandHandler(IEventRepository eventRepository, ILogger<GetAllEventsForUserCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetAllEventsForUserResponseDto>>> Handle(GetAllEventsForUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var events = await _eventRepository.GetAllEventsForUserAsync(request.UserId);

                if (events == null)
                {
                    _logger.LogWarning("No events found for user with ID: {UserId}", request.UserId);
                    return ApiResponse<List<GetAllEventsForUserResponseDto>>.Error(404, "No events found for the user.", null);
                }

                var eventDtos = events
                .OrderBy(e => e.FromDate)
                .Select(e => new GetAllEventsForUserResponseDto
                {
                    EventId = e.Id,
                    Title = e.Detail.Title,
                    Description = e.Detail.Description,
                    HasAttachment = e.Detail.HasAttachment,
                    HasTag = e.Detail.HasTag,
                    CreatedAt = e.Detail.CreatedAt,
                    UpdatedAt = e.Detail.UpdatedAt,
                    CategoryName = e.Category.Name,
                    CategoryColor = e.Category.Color,
                    FromDate = e.FromDate,
                    ToDate = e.ToDate,
                    Location = e.Location,
                    IsWholeDay = e.IsWholeDay,
                    RepeatId = e.RepeatId,
                    Priority = e.Priority.ToString(),
                    Status = e.Status.ToString(),
                    PriorityScore = e.PriorityScore,
                    Tags = e.TagLinks.Select(tl => tl.Tag.Name).ToList()
                }).ToList();

                _logger.LogInformation("Successfully retrieved events for user with ID: {UserId}", request.UserId);
                return ApiResponse<List<GetAllEventsForUserResponseDto>>.Success(200, "Events retrieved successfully.", eventDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving events for user with ID: {UserId}", request.UserId);
                return ApiResponse<List<GetAllEventsForUserResponseDto>>.Error(500, "An error occurred while retrieving events.", null);
            }
        }
    }
}