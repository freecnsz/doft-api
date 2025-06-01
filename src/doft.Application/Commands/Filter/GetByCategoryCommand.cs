using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Filter;
using doft.Application.Interfaces;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Application.Mappers.Detail;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Filter
{
    public class GetByCategoryCommand : IRequest<ApiResponse<List<GetAllItemForUserResponseDto>>>
    {
        public int CategoryId { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public string? ItemType { get; set; }

        public GetByCategoryCommand(int categoryId, string userId, string? itemType = null)
        {
            CategoryId = categoryId;
            UserId = userId;
            ItemType = itemType;
        }
    }

    public class GetByCategoryCommandHandler : IRequestHandler<GetByCategoryCommand, ApiResponse<List<GetAllItemForUserResponseDto>>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetByCategoryCommandHandler> _logger;

        public GetByCategoryCommandHandler(ITaskRepository taskRepository, IEventRepository eventRepository, ILogger<GetByCategoryCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetAllItemForUserResponseDto>>> Handle(GetByCategoryCommand request, CancellationToken cancellationToken)
        {
            var response = new List<GetAllItemForUserResponseDto>();

            // If ItemType is not specified or is "task", get tasks
            if (string.IsNullOrEmpty(request.ItemType) || request.ItemType.Equals("task", StringComparison.OrdinalIgnoreCase))
            {
                var tasks = await _taskRepository.GetTasksByCategory(request.CategoryId, request.UserId);
                if (tasks != null)
                {
                    response.AddRange(tasks.Select(task => new GetAllItemForUserResponseDto
                    {
                        ItemId = task.Id,
                        ItemType = task.Detail.ItemType.ToString(),
                        Title = task.Detail.Title,
                        Description = task.Detail.Description,
                        HasAttachment = task.Detail.HasAttachment,
                        HasTag = task.Detail.HasTag,
                        CreatedAt = task.Detail.CreatedAt,
                        UpdatedAt = task.Detail.UpdatedAt,
                        CategoryName = task.Category.Name,
                        CategoryColor = task.Category.Color,
                        DueDate = task.DueDate,
                        Priority = task.Priority.ToString(),
                        PriorityScore = task.PriorityScore,
                        Status = task.Status.ToString(),
                        Tags = task.TagLinks.Select(tl => tl.Tag.Name).ToList()
                    }));
                }
            }

            // If ItemType is not specified or is "event", get events
            if (string.IsNullOrEmpty(request.ItemType) || request.ItemType.Equals("event", StringComparison.OrdinalIgnoreCase))
            {
                var events = await _eventRepository.GetEventsByCategory(request.CategoryId, request.UserId);
                if (events != null)
                {
                    response.AddRange(events.Select(eventItem => new GetAllItemForUserResponseDto
                    {
                        ItemId = eventItem.Id,
                        ItemType = eventItem.Detail.ItemType.ToString(),
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
                        Tags = eventItem.TagLinks.Select(tl => tl.Tag.Name).ToList()
                    }));
                }
            }

            if (response.Count == 0)
            {
                _logger.LogWarning("No items found for category {CategoryId}", request.CategoryId);
                return ApiResponse<List<GetAllItemForUserResponseDto>>.NotFound("No items found for this category");
            }

            return ApiResponse<List<GetAllItemForUserResponseDto>>.Success(200, "Items retrieved successfully.", response);
        }
    }
}