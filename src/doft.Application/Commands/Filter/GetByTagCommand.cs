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
    public class GetByTagCommand : IRequest<ApiResponse<List<GetAllItemForUserResponseDto>>>
    {
        public string TagName { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public string? ItemType { get; set; }

        public GetByTagCommand(string tagName, string userId, string? itemType = null)
        {
            TagName = tagName;
            UserId = userId;
            ItemType = itemType;
        }
    }

    public class GetByTagCommandHandler : IRequestHandler<GetByTagCommand, ApiResponse<List<GetAllItemForUserResponseDto>>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetByTagCommandHandler> _logger;

        public GetByTagCommandHandler(ITagRepository tagRepository, ITagLinkRepository tagLinkRepository, ITaskRepository taskRepository, IEventRepository eventRepository, ILogger<GetByTagCommandHandler> logger)
        {
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _taskRepository = taskRepository;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetAllItemForUserResponseDto>>> Handle(GetByTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetTagByNameAsync(request.TagName);

            if (tag == null)
            {
                _logger.LogWarning("Tag not found: {TagName}", request.TagName);
                return ApiResponse<List<GetAllItemForUserResponseDto>>.NotFound("Tag not found");
            }

            var tagLinks = await _tagLinkRepository.GetAllItemsByTagAsync(tag.Id);
            var response = new List<GetAllItemForUserResponseDto>();

            foreach (var tagLink in tagLinks)
            {
                // If ItemType is not specified or matches the current item type
                if (string.IsNullOrEmpty(request.ItemType) || 
                    (request.ItemType.Equals("task", StringComparison.OrdinalIgnoreCase) && tagLink.ItemType == ItemType.Task) ||
                    (request.ItemType.Equals("event", StringComparison.OrdinalIgnoreCase) && tagLink.ItemType == ItemType.Event))
                {
                    if (tagLink.ItemType == ItemType.Task)
                    {
                        var task = await _taskRepository.GetByIdAsync(tagLink.ItemId);
                        if (task != null && task.OwnerId == request.UserId)
                        {
                            response.Add(new GetAllItemForUserResponseDto
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
                            });
                        }
                    }
                    else if (tagLink.ItemType == ItemType.Event)
                    {
                        var eventItem = await _eventRepository.GetByIdAsync(tagLink.ItemId);
                        if (eventItem != null && eventItem.OwnerId == request.UserId)
                        {
                            response.Add(new GetAllItemForUserResponseDto
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
                            });
                        }
                    }
                }
            }

            if (response.Count == 0)
            {
                _logger.LogWarning("No items found for tag {TagName}", request.TagName);
                return ApiResponse<List<GetAllItemForUserResponseDto>>.NotFound("No items found for this tag");
            }

            return ApiResponse<List<GetAllItemForUserResponseDto>>.Success(200, "Items retrieved successfully.", response);
        }
    }
}