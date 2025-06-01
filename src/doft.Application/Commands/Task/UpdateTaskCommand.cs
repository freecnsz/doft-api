using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Task;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Task
{
    public class UpdateTaskCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime? DueDate { get; set; }
        public DoftTaskStatus? Status { get; set; }
        public DoftTaskPriority? Priority { get; set; }
        public int? RepeatId { get; set; }
        public List<string> Tags { get; set; }

        public UpdateTaskCommand(int id, string ownerId, string title, string description, string categoryName, string categoryColor, DateTime? dueDate, DoftTaskStatus? status, DoftTaskPriority? priority, int? repeatId, List<string> tags)
        {
            Id = id;
            OwnerId = ownerId;
            Title = title;
            Description = description;
            CategoryName = categoryName;
            CategoryColor = categoryColor;
            DueDate = dueDate;
            Status = status;
            Priority = priority;
            RepeatId = repeatId;
            Tags = tags;
        }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, ApiResponse<bool>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;

        public UpdateTaskCommandHandler(
            ITaskRepository taskRepository,
            IDetailRepository detailRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITagLinkRepository tagLinkRepository,
            ILogger<UpdateTaskCommandHandler> logger)
        {
            _taskRepository = taskRepository;
            _detailRepository = detailRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(request.Id);

                if (task == null || task.OwnerId != request.OwnerId)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found or unauthorized", request.Id);
                    return ApiResponse<bool>.NotFound("Task not found or unauthorized access");
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
                    task.CategoryId = category.Id;
                }

                // Update task details with null checks
                task.Detail.Title = request.Title ?? task.Detail.Title;
                task.Detail.Description = request.Description ?? task.Detail.Description;
                task.Detail.UpdatedAt = DateTime.UtcNow;
                task.DueDate = request.DueDate ?? task.DueDate;
                task.Status = request.Status ?? task.Status;
                task.Priority = request.Priority ?? task.Priority;
                task.RepeatId = request.RepeatId ?? task.RepeatId;

                // Handle tags: only add new tags, do not remove existing ones
                if (request.Tags != null && request.Tags.Any())
                {
                    task.Detail.HasTag = true;

                    // Get existing tag links for this task
                    var existingTagLinks = await _tagLinkRepository.GetTagLinksByItemAsync(ItemType.Task, task.Detail.Id);
                    var existingTagNames = existingTagLinks.Select(tl => tl.Tag.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    // Add only new tags that are not already linked
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
                                ItemId = task.Detail.Id
                            };
                            await _tagLinkRepository.AddAsync(tagLink);
                        }
                    }
                }

                await _taskRepository.UpdateAsync(task);

                // Get current category for response
                var currentCategory = await _categoryRepository.GetByIdAsync(task.CategoryId);
                var currentTags = await _tagLinkRepository.GetTagLinksByItemAsync(ItemType.Task, task.Detail.Id);
                var tagNames = currentTags.Select(tl => tl.Tag.Name).ToList();

                _logger.LogInformation("Task with ID {TaskId} updated successfully", request.Id);
                return ApiResponse<bool>.Success(200, "Task updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID {TaskId}", request.Id);
                return ApiResponse<bool>.Error(500, "An error occurred while updating the task", false);
            }
        }
    }
}