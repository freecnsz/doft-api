using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Note;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NoteController> _logger;

        public NoteController(IMediator mediator, ILogger<NoteController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddNote([FromBody] AddNoteCommand command)
        {
            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _mediator.Send(command);

            if (response == null)
            {
                _logger.LogError("Failed to add note for user {UserId}", command.OwnerId);
                return BadRequest(new { Message = "Failed to add note" });
            }

            _logger.LogInformation("Note added successfully for user {UserId}", command.OwnerId);
            return Ok(response);
        }

        [HttpGet("getAllForUser")]
        [Authorize]
        public async Task<IActionResult> GetAllNotesForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetAllNotesForUserCommand(userId);
            var response = await _mediator.Send(query);

            if (response == null)
            {
                _logger.LogWarning("No notes found for user {UserId}", userId);
                return NotFound(new { Message = "No notes found" });
            }

            _logger.LogInformation("Retrieved notes for user {UserId}", userId);
            return Ok(response);
        }

        [HttpGet("getById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _mediator.Send(new GetNoteByIdCommand(id, userId));

            if (response == null)
            {
                _logger.LogWarning("Note with ID {NoteId} not found for user {UserId}", id, userId);
                return NotFound(new { Message = "Note not found" });
            }

            _logger.LogInformation("Retrieved note with ID {NoteId} for user {UserId}", id, userId);
            return Ok(response);
        }

        [HttpGet("getByCategory/{categoryId}")]
        [Authorize]

        public async Task<IActionResult> GetNotesByCategory(int categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _mediator.Send(new GetNotesByCategory(categoryId, userId));

            if (response == null)
            {
                _logger.LogWarning("No notes found for category ID {CategoryId} for user {UserId}", categoryId, userId);
                return NotFound(new { Message = "No notes found for the specified category" });
            }

            _logger.LogInformation("Retrieved notes for category ID {CategoryId} for user {UserId}", categoryId, userId);
            return Ok(response);
        }


        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote([FromRoute] int id, [FromBody] UpdateNoteCommand command)
        {
            command.Id = id;
            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _mediator.Send(command);

            if (response == null)
            {
                _logger.LogError("Failed to update note with ID {NoteId}", command.Id);
                return BadRequest(new { Message = "Failed to update note" });
            }

            _logger.LogInformation("Note with ID {NoteId} updated successfully", command.Id);
            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var command = new DeleteNoteCommand(id);
            
            var response = await _mediator.Send(command);

            if (response == null)
            {
                _logger.LogError("Failed to delete note with ID {NoteId}", id);
                return BadRequest(new { Message = "Failed to delete note" });
            }

            _logger.LogInformation("Note with ID {NoteId} deleted successfully", id);
            return Ok(response);
        }
    }
}