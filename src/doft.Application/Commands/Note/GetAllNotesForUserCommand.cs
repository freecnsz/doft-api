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
    public class GetAllNotesForUserCommand(string userId) : IRequest<ApiResponse<List<GetAllForUserResponseDto>>>
    {
        [JsonIgnore]
        public string UserId { get; set; } = userId;
    }

    public class GetAllNotesForUserCommandHandler : IRequestHandler<GetAllNotesForUserCommand, ApiResponse<List<GetAllForUserResponseDto>>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly ILogger<GetAllNotesForUserCommandHandler> _logger;

        public GetAllNotesForUserCommandHandler(INoteRepository noteRepository, ILogger<GetAllNotesForUserCommandHandler> logger)
        {
            _logger = logger;
            _noteRepository = noteRepository;
        }

        public async Task<ApiResponse<List<GetAllForUserResponseDto>>> Handle(GetAllNotesForUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notes = await _noteRepository.GetAllNotesForUserAsync(request.UserId, cancellationToken);
                
                if (notes == null || !notes.Any())
                {
                    _logger.LogInformation("No notes found for user {UserId}", request.UserId);
                    return ApiResponse<List<GetAllForUserResponseDto>>.Success(200, "No notes found", new List<GetAllForUserResponseDto>());
                }

                var response = notes
                    .OrderBy(n => n.Detail.CreatedAt)
                    .Select(note => new GetAllForUserResponseDto
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

                _logger.LogInformation("Retrieved {NoteCount} notes for user {UserId}", response.Count, request.UserId);
                return ApiResponse<List<GetAllForUserResponseDto>>.Success(200, "Notes retrieved successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notes for user {UserId}", request.UserId);
                return ApiResponse<List<GetAllForUserResponseDto>>.Error(500, "An error occurred while retrieving notes", null);
            }
        }
    }
}