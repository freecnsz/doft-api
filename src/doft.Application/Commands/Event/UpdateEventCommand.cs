using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Event;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Event
{
    public class UpdateEventCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Location { get; set; }
        public bool? IsAllDay { get; set; }
        public int? RepeatId { get; set; }
        public List<string> Tags { get; set; }

        public UpdateEventCommand(int id, string ownerId, string title, string description, string categoryName, string categoryColor, DateTime? fromDate, DateTime? toDate, string location, bool? isAllDay, int? repeatId, List<string> tags)
        {
            Id = id;
            OwnerId = ownerId;
            Title = title;
            Description = description;
            CategoryName = categoryName;
            CategoryColor = categoryColor;
            FromDate = fromDate;
            ToDate = toDate;
            Location = location;
            IsAllDay = isAllDay;
            RepeatId = repeatId;
            Tags = tags;
        }
    }

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ApiResponse<bool>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ILogger<UpdateEventCommandHandler> _logger;

        public UpdateEventCommandHandler(
            IEventRepository eventRepository,
            IDetailRepository detailRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITagLinkRepository tagLinkRepository,
            ILogger<UpdateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _detailRepository = detailRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var eventEntity = await _eventRepository.GetByIdAsync(request.Id);

                if (eventEntity == null || eventEntity.OwnerId != request.OwnerId)
                {
                    _logger.LogWarning("Event with ID {EventId} not found or unauthorized", request.Id);
                    return ApiResponse<bool>.NotFound("Event not found or you do not have permission to update it.");
                }

                // Handle category if provided
                if (!string.IsNullOrEmpty(request.CategoryName))
                {
                    var category = await _categoryRepository.GetByNameAsync(request.CategoryName);
                    if (category == null)
                    {
                        category = new Domain.Entities.Category
                        {
                            Name = request.CategoryName,
                            Color = request.CategoryColor ?? "#000000",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        category = await _categoryRepository.AddAsync(category);

                        // Create user-category relationship
                        var userCategory = new UserCategory
                        {
                            UserId = request.OwnerId,
                            CategoryId = category.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _categoryRepository.AddUserCategoryAsync(userCategory);
                    }
                    else
                    {
                        // Check if user already has this category
                        var userCategory = await _categoryRepository.GetUserCategoryAsync(category.Id, request.OwnerId);
                        if (userCategory == null)
                        {
                            // Create user-category relationship
                            userCategory = new UserCategory
                            {
                                UserId = request.OwnerId,
                                CategoryId = category.Id,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            await _categoryRepository.AddUserCategoryAsync(userCategory);
                        }
                    }
                    eventEntity.CategoryId = category.Id;
                }

                // Update event details with null checks
                eventEntity.Detail.Title = request.Title ?? eventEntity.Detail.Title;
                eventEntity.Detail.Description = request.Description ?? eventEntity.Detail.Description;
                eventEntity.Detail.UpdatedAt = DateTime.UtcNow;
                eventEntity.FromDate = request.FromDate ?? eventEntity.FromDate;
                eventEntity.ToDate = request.ToDate ?? eventEntity.ToDate;
                eventEntity.Location = request.Location ?? eventEntity.Location;
                eventEntity.IsWholeDay = request.IsAllDay ?? eventEntity.IsWholeDay;
                eventEntity.RepeatId = request.RepeatId ?? eventEntity.RepeatId;

                // Handle tags: add only new tags to existing ones
                if (request.Tags != null && request.Tags.Any())
                {
                    eventEntity.Detail.HasTag = true;

                    // Get existing tag links
                    var existingTagLinks = await _tagLinkRepository.GetTagLinksByItemAsync(ItemType.Event, eventEntity.Detail.Id);
                    var existingTagNames = existingTagLinks.Select(tl => tl.Tag.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    // Add only new tags
                    foreach (var tagName in request.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        if (!existingTagNames.Contains(tagName))
                        {
                            var tag = await _tagRepository.GetTagByNameAsync(tagName);
                            if (tag == null)
                            {
                                tag = new Tag { Name = tagName };
                                tag = await _tagRepository.AddAsync(tag);
                            }

                            var tagLink = new TagLink
                            {
                                TagId = tag.Id,
                                ItemId = eventEntity.Detail.Id
                            };
                            await _tagLinkRepository.AddAsync(tagLink);
                        }
                    }
                }

                await _eventRepository.UpdateAsync(eventEntity);

                // Get current category and tags for response
                var currentCategory = await _categoryRepository.GetByIdAsync(eventEntity.CategoryId);
                var currentTags = await _tagLinkRepository.GetTagLinksByItemAsync(ItemType.Event, eventEntity.Detail.Id);
                var tagNames = currentTags.Select(tl => tl.Tag.Name).ToList();

                _logger.LogInformation("Event with ID {EventId} updated successfully", request.Id);
                return ApiResponse<bool>.Success(200, "Event updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with ID {EventId}", request.Id);
                return ApiResponse<bool>.Error(500, "An error occurred while updating the event", false);
            }
        }
    }
}