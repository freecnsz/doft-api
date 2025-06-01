using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Event;
using doft.Application.Commands.Task;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController(IMediator mediator, ILogger<EventController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<EventController> _logger = logger;

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddEvent([FromBody] AddEventCommand command)
        {

            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to add event.");
                return BadRequest("Failed to add event.");
            }

            _logger.LogInformation("Event added successfully.");
            return Ok(result);
        }

        [HttpGet("getAllForUser")]
        [Authorize]
        public async Task<IActionResult> GetAllForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetAllEventsForUserCommand(userId));

            if (result == null)
            {
                _logger.LogError("No events found for user {UserId}", userId);
                return NotFound("No events found for the user.");
            }

            _logger.LogInformation("Events retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpGet("getById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetEventById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetEventByIdCommand(id, userId));

            if (result == null)
            {
                _logger.LogError("Event with ID {EventId} not found", id);
                return NotFound($"Event with ID {id} not found.");
            }

            _logger.LogInformation("Event with ID {EventId} retrieved successfully", id);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventCommand command, [FromRoute] int id)
        {
            command.Id = id;
            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update event.");
                return BadRequest("Failed to update event.");
            }

            _logger.LogInformation("Event updated successfully.");
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _mediator.Send(new DeleteEventCommand(id));

            if (result == null)
            {
                _logger.LogError("Failed to delete event with ID {EventId}", id);
                return BadRequest("Failed to delete event.");
            }

            _logger.LogInformation("Event with ID {EventId} deleted successfully", id);
            return Ok(result);
        }
    }
}