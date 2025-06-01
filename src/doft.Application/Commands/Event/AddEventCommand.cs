using System.Text.Json.Serialization;
using doft.Application.DTOs;
using doft.Application.DTOs.Event;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Event
{
    public class AddEventCommand : IRequest<ApiResponse<AddEventResponseDto>>
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Location { get; set; }
        public bool IsWholeDay { get; set; }
        public int RepeatId { get; set; }
        public List<string> Tags { get; set; }

        public AddEventCommand(string title, string description, string categoryName, string categoryColor, DateTime fromDate, DateTime toDate, string location, bool isWholeDay, int repeatId,List<string> tags)
        {
            Title = title;
            Description = description;
            CategoryName = categoryName ?? string.Empty;
            CategoryColor = categoryColor ?? string.Empty;
            FromDate = fromDate;
            ToDate = toDate;
            Location = location ?? string.Empty;
            IsWholeDay = isWholeDay;
            RepeatId = repeatId;
            Tags = tags ?? [];
        }
    }

    public class AddEventCommandHandler : IRequestHandler<AddEventCommand, ApiResponse<AddEventResponseDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ILogger<AddEventCommandHandler> _logger;

        public AddEventCommandHandler(IEventRepository eventRepository, IDetailRepository detailRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository, ITagLinkRepository tagLinkRepository, ILogger<AddEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _detailRepository = detailRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<AddEventResponseDto>> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryRepository.GetByNameAsync(request.CategoryName);

                if (category == null)
                {
                    // Create new category
                    category = new Domain.Entities.Category
                    {
                        Name = request.CategoryName,
                        Color = request.CategoryColor,
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

                // Create a new Detail entity
                var detail = new Detail
                {
                    ItemType = ItemType.Event,
                    Title = request.Title,
                    Description = request.Description,
                    HasAttachment = false,
                    HasTag = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                // Add the Detail entity to the repository
                var createdDetail = await _detailRepository.AddAsync(detail);

                // Create a new Event entity
                var eventEntity = new Domain.Entities.Event
                {
                    OwnerId = request.OwnerId,
                    CategoryId = category.Id,
                    DetailId = detail.Id,
                    Location = request.Location,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    IsWholeDay = request.IsWholeDay,
                    RepeatId =request.RepeatId
                };

                // Add the Event entity to the repository
                var createdEvent = await _eventRepository.AddAsync(eventEntity);
                createdDetail.ItemId = createdEvent.Id;

                // Add tags to the event if provided
                if (request.Tags != null && request.Tags.Count > 0)
                {
                    var tagIds = new HashSet<int>();
                    foreach (var tagName in request.Tags.Select(t => t.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrWhiteSpace(tagName)) continue;

                        var tag = await _tagRepository.GetTagByNameAsync(tagName);
                        if (tag == null)
                        {
                            tag = new Tag { Name = tagName };
                            tag = await _tagRepository.AddAsync(tag);
                        }

                        if (!tagIds.Contains(tag.Id))
                        {
                            var tagLink = new TagLink
                            {
                                ItemType = ItemType.Event,
                                ItemId = createdEvent.Id,
                                TagId = tag.Id,
                            };
                            await _tagLinkRepository.AddAsync(tagLink);
                            tagIds.Add(tag.Id);
                        }
                    }
                    createdDetail.HasTag = true;
                }

                // Update the Detail entity with the new ItemId
                createdDetail.UpdatedAt = DateTime.UtcNow;
                await _detailRepository.UpdateAsync(createdDetail); 

                _logger.LogInformation("Event added successfully.");
                return ApiResponse<AddEventResponseDto>.Success(200, "Event added successfully", new AddEventResponseDto
                {
                    EventId = eventEntity.Id,
                    CreatedAt = detail.CreatedAt
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding event.");
                return ApiResponse<AddEventResponseDto>.Error(500, "Internal server error", null);
            }

        }
    }

}