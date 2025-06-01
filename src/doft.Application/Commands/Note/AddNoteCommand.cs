using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using doft.Application.DTOs;
using doft.Application.DTOs.Note;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Note
{
    public class AddNoteCommand : IRequest<ApiResponse<AddNoteResponseDto>>
    {
        [JsonIgnore]
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public List<string> Tags { get; set; }

        public AddNoteCommand(
            string title,
            string description,
            string content,
            string categoryName,
            string categoryColor,
            List<string>? tags
        )
        {
            Title = title;
            Description = description;
            Content = content;
            CategoryName = categoryName ?? string.Empty;
            CategoryColor = categoryColor ?? string.Empty;
            Tags = tags ?? new List<string>();
        }
    }

    public class AddNoteCommandHandler : IRequestHandler<AddNoteCommand, ApiResponse<AddNoteResponseDto>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IDetailRepository _detailRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITagLinkRepository _tagLinkRepository;
        private readonly ILogger<AddNoteCommandHandler> _logger;

        public AddNoteCommandHandler(
            INoteRepository noteRepository,
            IDetailRepository detailRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITagLinkRepository tagLinkRepository,
            ILogger<AddNoteCommandHandler> logger)
        {
            _noteRepository = noteRepository;
            _detailRepository = detailRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _tagLinkRepository = tagLinkRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<AddNoteResponseDto>> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create a new Detail entity first
                var detail = new Detail
                {
                    ItemType = ItemType.Note,
                    Title = request.Title,
                    Description = request.Description,
                    HasAttachment = false,
                    HasTag = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Add the Detail entity to the repository
                var createdDetail = await _detailRepository.AddAsync(detail);

                // Create Note entity
                var note = new Domain.Entities.Note
                {
                    OwnerId = request.OwnerId,
                    DetailId = createdDetail.Id,
                    Content = request.Content
                };

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

                // Add the Note entity to the repository
                var createdNote = await _noteRepository.AddAsync(note);
                createdDetail.ItemId = createdNote.Id;

                // Add tags to the note if provided
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
                                ItemType = ItemType.Note,
                                ItemId = createdNote.Id,
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

                // Return success response with created note details
                return ApiResponse<AddNoteResponseDto>.Success(200, "Note added successfully", new AddNoteResponseDto
                {
                    Id = createdNote.Id.ToString(),
                    CreatedAt = createdDetail.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding note");
                return ApiResponse<AddNoteResponseDto>.Error(500, "An error occurred while adding the note", null);
            }
        }
    }
}