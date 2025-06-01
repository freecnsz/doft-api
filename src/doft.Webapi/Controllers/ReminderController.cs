using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Reminder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReminderController> _logger;

        public ReminderController(IMediator mediator, ILogger<ReminderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddReminder([FromBody] AddReminderCommand command)
        {
            command.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to add reminder for user {UserId}", command.UserId);
                return BadRequest(new { Message = "Failed to add reminder" });
            }

            _logger.LogInformation("Reminder added successfully for user {UserId}", command.UserId);
            return Ok(result);
        }

        [HttpGet("getByTaskId/{taskId}")]
        [Authorize]
        public async Task<IActionResult> GetRemindersByTaskId(int taskId)
        {
            var result = await _mediator.Send(new GetRemindersByTaskIdCommand(taskId));

            if (result == null)
            {
                _logger.LogError("No reminders found for task {TaskId}", taskId);
                return NotFound(new { Message = "No reminders found for this task" });
            }

            _logger.LogInformation("Reminders retrieved successfully for task {TaskId}", taskId);
            return Ok(result);
        }

        [HttpGet("getByUserId")]
        [Authorize]
        public async Task<IActionResult> GetRemindersByUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetRemindersByUserIdCommand(userId));

            if (result == null)
            {
                _logger.LogError("No reminders found for user {UserId}", userId);
                return NotFound(new { Message = "No reminders found for this user" });
            }

            _logger.LogInformation("Reminders retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReminder([FromBody] UpdateReminderCommand command, [FromRoute] int id)
        {
            command.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.Id = id;
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update reminder with ID {ReminderId}", command.Id);
                return BadRequest(new { Message = "Failed to update reminder" });
            }

            _logger.LogInformation("Reminder with ID {ReminderId} updated successfully", command.Id);
            return Ok(result);
        }
        

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReminder([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteReminderCommand(id));

            if (result == null)
            {
                _logger.LogError("Failed to delete reminder with ID {ReminderId}", id);
                return BadRequest(new { Message = "Failed to delete reminder" });
            }

            _logger.LogInformation("Reminder with ID {ReminderId} deleted successfully", id);
            return Ok(result);
        }
    }

}
