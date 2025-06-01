using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Note;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Note
{
    public class UpdateNoteCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public List<string> Tags { get; set; }

        public UpdateNoteCommand(int id, string ownerId, string title, string description, string content, string categoryName, string categoryColor, List<string> tags)
        {
            Id = id;
            OwnerId = ownerId;
            Title = title;
            Description = description;
            Content = content;
            CategoryName = categoryName;
            CategoryColor = categoryColor;
            Tags = tags;
        }
    }

    public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, ApiResponse<bool>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ILogger<UpdateNoteCommandHandler> _logger;

        public UpdateNoteCommandHandler(
            INoteRepository noteRepository,
            IDetailRepository detailRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITagLinkRepository tagLinkRepository,
            ILogger<UpdateNoteCommandHandler> logger)
        {
            _noteRepository = noteRepository;
            _detailRepository = detailRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var note = await _noteRepository.GetByIdAsync(request.Id);

                if (note == null || note.OwnerId != request.OwnerId)
                {
                    _logger.LogWarning("Note with ID {NoteId} not found or unauthorized", request.Id);
                    return ApiResponse<bool>.NotFound("Note not found or unauthorized access");
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
                    note.CategoryId = category.Id;
                }

                // Update note details with null checks
                note.Detail.Title = request.Title ?? note.Detail.Title;
                note.Detail.Description = request.Description ?? note.Detail.Description;
                note.Detail.UpdatedAt = DateTime.UtcNow;
                note.Content = request.Content ?? note.Content;

                // Handle tags: add only new tags to existing ones
                if (request.Tags != null && request.Tags.Any())
                {
                    note.Detail.HasTag = true;

                    // Get existing tag links
                    var existingTagLinks = await _tagLinkRepository.GetTagLinksByItemAsync(ItemType.Note, note.Id);
                    var existingTagNames = existingTagLinks.Select(tl => tl.Tag.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    // Find new tags that are not already linked
                    var newTags = request.Tags
                        .Where(tagName => !existingTagNames.Contains(tagName, StringComparer.OrdinalIgnoreCase))
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var tagName in newTags)
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
                            ItemId = note.Id,
                            ItemType = ItemType.Note
                        };
                        await _tagLinkRepository.AddAsync(tagLink);
                    }
                }

                await _noteRepository.UpdateAsync(note);

                // Get current category and tags for response
                var currentCategory = await _categoryRepository.GetByIdAsync(note.CategoryId);
                var currentTags = await _tagLinkRepository.GetTagLinksByItemAsync(ItemType.Note, note.Id);
                var tagNames = currentTags.Select(tl => tl.Tag.Name).ToList();

                _logger.LogInformation("Note with ID {NoteId} updated successfully", request.Id);
                return ApiResponse<bool>.Success(200, "Note updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating note with ID {NoteId}", request.Id);
                return ApiResponse<bool>.Error(500, "An error occurred while updating the note", false);
            }
        }
    }
}