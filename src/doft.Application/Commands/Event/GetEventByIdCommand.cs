using System;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Event;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Event
{
    public class GetEventByIdCommand : IRequest<ApiResponse<GetEventByIdResponseDto>>
    {
        public int EventId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string UserId { get; set; }

        public GetEventByIdCommand(int eventId, string userId)
        {
            EventId = eventId;
            UserId = userId;
        }
    }

    public class GetEventByIdCommandHandler : IRequestHandler<GetEventByIdCommand, ApiResponse<GetEventByIdResponseDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetEventByIdCommandHandler> _logger;

        public GetEventByIdCommandHandler(IEventRepository eventRepository, ILogger<GetEventByIdCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<GetEventByIdResponseDto>> Handle(GetEventByIdCommand request, CancellationToken cancellationToken)
        {
            var eventItem = await _eventRepository.GetByIdAsync(request.EventId);

            if (eventItem == null || eventItem.OwnerId != request.UserId)
            {
                _logger.LogWarning("Event not found with ID: {EventId}", request.EventId);
                return ApiResponse<GetEventByIdResponseDto>.NotFound("Event not found");
            }


            var response = new GetEventByIdResponseDto
            {
                EventId = eventItem.Id,
                Title = eventItem.Detail.Title,
                Description = eventItem.Detail.Description,
                HasAttachment = eventItem.Detail.HasAttachment,
                HasTag = eventItem.Detail.HasTag,
                CreatedAt = eventItem.Detail.CreatedAt,
                UpdatedAt = eventItem.Detail.UpdatedAt,
                CategoryName = eventItem.Category.Name,
                CategoryColor = eventItem.Category.Color,
                FromDate = eventItem.FromDate,
                ToDate = eventItem.ToDate,
                Location = eventItem.Location,
                IsWholeDay = eventItem.IsWholeDay,
                Status = eventItem.Status.ToString(),
                Priority = eventItem.Priority.ToString(),
                PriorityScore = eventItem.PriorityScore,
                Tags = eventItem.TagLinks.Select(tl => tl.Tag.Name).ToList()
            };

            _logger.LogInformation("Successfully retrieved event {EventId} for user {UserId}", request.EventId, request.UserId);
            return ApiResponse<GetEventByIdResponseDto>.Success(200, "Event retrieved successfully", response);
        }
    }
} 