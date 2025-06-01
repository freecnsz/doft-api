using System;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Note;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Note
{
    public class GetNoteByIdCommand : IRequest<ApiResponse<GetNoteByIdResponseDto>>
    {
        public int NoteId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string UserId { get; set; }

        public GetNoteByIdCommand(int noteId, string userId)
        {
            NoteId = noteId;
            UserId = userId;
        }
    }

    public class GetNoteByIdCommandHandler : IRequestHandler<GetNoteByIdCommand, ApiResponse<GetNoteByIdResponseDto>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly ILogger<GetNoteByIdCommandHandler> _logger;

        public GetNoteByIdCommandHandler(INoteRepository noteRepository, ILogger<GetNoteByIdCommandHandler> logger)
        {
            _noteRepository = noteRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<GetNoteByIdResponseDto>> Handle(GetNoteByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var note = await _noteRepository.GetByIdAsync(request.NoteId);

                if (note == null)
                {
                    _logger.LogWarning("Note not found with ID: {NoteId}", request.NoteId);
                    return ApiResponse<GetNoteByIdResponseDto>.NotFound("Note not found");
                }

                var response = new GetNoteByIdResponseDto
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
                };

                _logger.LogInformation("Successfully retrieved note {NoteId} for user {UserId}", request.NoteId, request.UserId);
                return ApiResponse<GetNoteByIdResponseDto>.Success(200, "Note retrieved successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving note with ID: {NoteId}", request.NoteId);
                return ApiResponse<GetNoteByIdResponseDto>.Error(500, "An error occurred while retrieving the note", null);
            }
        }
    }
} 