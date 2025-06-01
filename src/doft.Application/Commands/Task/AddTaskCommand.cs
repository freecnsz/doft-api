using System.Text.Json.Serialization;
using doft.Application.DTOs;
using doft.Application.DTOs.Task;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace doft.Application.Commands.Task
{
    public class AddTaskCommand : IRequest<ApiResponse<AddTaskResponseDto>>
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public int RepeatId { get; set; }
        public DateTime DueDate { get; set; }
        public List<string> Tags { get; set; }

        public AddTaskCommand(
            string title,
            string description,
            string categoryName,
            string categoryColor,
            int repeatId,
            DateTime dueDate,
            List<string> tags
        )
        {
            Title = title;
            Description = description;
            CategoryName = categoryName ?? string.Empty;
            CategoryColor = categoryColor ?? string.Empty;
            RepeatId = repeatId;
            DueDate = dueDate;
            Tags = tags ?? new List<string>();
        }
    }

    public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, ApiResponse<AddTaskResponseDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ILogger<AddTaskCommandHandler> _logger;

        public AddTaskCommandHandler(
            ITaskRepository taskRepository,
            IDetailRepository detailRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITagLinkRepository tagLinkRepository,
            ILogger<AddTaskCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _detailRepository = detailRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<AddTaskResponseDto>> Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Handle Category
                var category = await _categoryRepository.GetByNameAsync(request.CategoryName);
                if (category == null && !string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    // Create new category if it doesn't exist
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
                else if (category != null)
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

                // 2. Create Detail
                var newDetail = new Detail
                {
                    ItemType = ItemType.Task,
                    Title = request.Title,
                    Description = request.Description,
                    HasAttachment = false,
                    HasTag = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdDetail = await _detailRepository.AddAsync(newDetail);

                // 3. Create Task
                var newTask = new DoftTask
                {
                    OwnerId = request.OwnerId,
                    CategoryId = category.Id,
                    DetailId = createdDetail.Id,
                    Status = DoftTaskStatus.Pending,
                    Priority = DoftTaskPriority.Low, // Default priority, will be calculated later
                    RepeatId = request.RepeatId,
                    DueDate = request.DueDate
                };

                var createdTask = await _taskRepository.AddAsync(newTask);
                createdDetail.ItemId = createdTask.Id;

                // 4. Handle Tags
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
                                ItemType = ItemType.Task,
                                ItemId = createdTask.Id,
                                TagId = tag.Id,
                            };
                            await _tagLinkRepository.AddAsync(tagLink);
                            tagIds.Add(tag.Id);
                        }
                    }
                    createdDetail.HasTag = true;
                }

                createdDetail.UpdatedAt = DateTime.UtcNow;
                await _detailRepository.UpdateAsync(createdDetail);

                var response = new AddTaskResponseDto
                {
                    TaskId = createdTask.Id,
                    CreatedAt = createdDetail.CreatedAt,
                    Title = createdDetail.Title,
                    Description = createdDetail.Description,
                    DueDate = createdTask.DueDate,
                    CategoryId = createdTask.CategoryId,
                    Status = createdTask.Status,
                };

                _logger.LogInformation("Task added successfully. TaskId: {TaskId}, CreatedAt: {CreatedAt}", response.TaskId, response.CreatedAt);
                return ApiResponse<AddTaskResponseDto>.Success(200, "Task added successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a task.");
                return ApiResponse<AddTaskResponseDto>.Error(500, "An error occurred while adding the task.", null);
            }
        }
    }
}