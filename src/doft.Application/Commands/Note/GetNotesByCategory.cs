using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Note;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Note
{
    public class GetNotesByCategory : IRequest<ApiResponse<List<GetNoteByIdResponseDto>>>
    {
        public int CategoryId { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }

        public GetNotesByCategory(int categoryId, string userId)
        {
            CategoryId = categoryId;
            UserId = userId;
        }
    }

    public class GetNotesByCategoryHandler : IRequestHandler<GetNotesByCategory, ApiResponse<List<GetNoteByIdResponseDto>>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly ILogger<GetNotesByCategoryHandler> _logger;

        public GetNotesByCategoryHandler(INoteRepository noteRepository, ILogger<GetNotesByCategoryHandler> logger)
        {
            _noteRepository = noteRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<GetNoteByIdResponseDto>>> Handle(GetNotesByCategory request, CancellationToken cancellationToken)
        {
            try
            {
                var notes = await _noteRepository.GetNotesByCategoryAsync(request.CategoryId, request.UserId, cancellationToken);

                if (notes == null || !notes.Any())
                {
                    _logger.LogInformation("No notes found for category ID: {CategoryId} for user ID: {UserId}", request.CategoryId, request.UserId);
                    return ApiResponse<List<GetNoteByIdResponseDto>>.Success(200, "No notes found in this category", new List<GetNoteByIdResponseDto>());
                }

                var response = notes.Select(note => new GetNoteByIdResponseDto
                {
                    NoteId = note.Id,
                    Title = note.Detail.Title,
                    Description = note.Detail.Description,
                    HasAttachment = note.Detail.HasAttachment,
                    HasTag = note.Detail.HasTag,
                    CreatedAt = note.Detail.CreatedAt,
                    UpdatedAt = note.Detail.UpdatedAt,
                    CategoryName = note.Category?.Name ?? "Uncategorized",
                    CategoryColor = note.Category?.Color ?? "#000000",
                    Tags = note.TagLinks?.Select(tl => tl.Tag.Name).ToList() ?? new List<string>(),
                    Content = note.Content
                }).ToList();

                _logger.LogInformation("Retrieved {Count} notes for category ID {CategoryId} for user {UserId}", response.Count, request.CategoryId, request.UserId);
                return ApiResponse<List<GetNoteByIdResponseDto>>.Success(200, "Category notes retrieved successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notes for category ID: {CategoryId} for user ID: {UserId}", request.CategoryId, request.UserId);
                return ApiResponse<List<GetNoteByIdResponseDto>>.Error(500, "An error occurred while retrieving category notes", null);
            }
        }
    }
}