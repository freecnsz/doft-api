using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Preference;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferenceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PreferenceController> _logger;

        public PreferenceController(IMediator mediator, ILogger<PreferenceController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddPreference([FromBody] AddPreferenceCommand command)
        {
            command.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to add preference for user {UserId}", command.UserId);
                return BadRequest(new { Message = "Failed to add preference" });
            }

            _logger.LogInformation("Preference added successfully for user {UserId}", command.UserId);
            return Ok(result);
        }

        [HttpGet("getByUserId")]
        [Authorize]
        public async Task<IActionResult> GetPreferencesByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var result = await _mediator.Send(new GetPreferencesByUserIdCommand(userId));

            if (result == null)
            {
                _logger.LogWarning("No preferences found for user {UserId}", userId);
                return NotFound(new { Message = "No preferences found" });
            }

            _logger.LogInformation("Preferences retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePreference([FromBody] UpdatePreferenceCommand command, [FromRoute] int id)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update preference for preference ID {PreferenceId}", command.Id);
                return BadRequest(new { Message = "Failed to update preference" });
            }

            _logger.LogInformation("Preference updated successfully for preference ID {PreferenceId}", command.Id);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePreference([FromRoute] int id)
        {

            var result = await _mediator.Send(new DeletePreferenceCommand(id));

            if (result == null)
            {
                _logger.LogError("Failed to delete preference {PreferenceId}", id);
                return BadRequest(new { Message = "Failed to delete preference" });
            }

            _logger.LogInformation("Preference {PreferenceId} deleted successfully", id);
            return Ok(result);
        }



        
        
    }
}