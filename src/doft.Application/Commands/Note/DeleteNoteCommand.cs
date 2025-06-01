using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Note
{
    public class DeleteNoteCommand(int id) : IRequest<ApiResponse<bool>>
    {
        public int Id { get; set; } = id;
    }

    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, ApiResponse<bool>>
    {
        private readonly INoteRepository _noteRepository;
        private readonly ILogger<DeleteNoteCommandHandler> _logger;

        public DeleteNoteCommandHandler(INoteRepository noteRepository, ILogger<DeleteNoteCommandHandler> logger)
        {
            _noteRepository = noteRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var note = await _noteRepository.GetByIdAsync(request.Id);
    
                if (note == null)
                {
                    _logger.LogWarning("Note with ID {NoteId} not found", request.Id);
                    return ApiResponse<bool>.Error(404, "Note not found", false);
                }
    
                note.Detail.IsDeleted = true; // Mark the note as deleted
                await _noteRepository.UpdateAsync(note);

                _logger.LogInformation("Note with ID {NoteId} deleted successfully", request.Id);
                return ApiResponse<bool>.Success(200, "Note deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note with ID {NoteId}", request.Id);
                return ApiResponse<bool>.Error(500, "An error occurred while deleting the note", false);
            }
           
        }
    }
    
}