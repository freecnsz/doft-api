using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Schedule;
using doft.Application.Commands.Task;
using doft.Application.DTOs.Schedule;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using src.doft.Application.Commands.Task;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TaskController> _logger;

        public TaskController(IMediator mediator, ILogger<TaskController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddTask([FromBody] AddTaskCommand command)
        {
            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to add task for user {UserId}", command.OwnerId);
                return BadRequest(new { Message = "Failed to add task" });
            }

            _logger.LogInformation("Task added successfully for user {UserId}", command.OwnerId);
            return Ok(result);
        }

        [HttpGet("getAllForUser")]
        [Authorize]
        public async Task<IActionResult> GetAllForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetAllForUserCommand(userId));

            if (result == null)
            {
                _logger.LogError("No tasks found for user {UserId}", userId);
                return NotFound(new { Message = "No tasks found for the user" });
            }

            _logger.LogInformation("Tasks retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpGet("getById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id <= 0)
            {
                _logger.LogError("Invalid task ID: {TaskId}", id);
                return BadRequest(new { Message = "Invalid task ID" });
            }

            var result = await _mediator.Send(new GetTaskByIdCommand(id, userId));

            if (result == null)
            {
                _logger.LogError("Task with ID {TaskId} not found", id);
                return NotFound(new { Message = "Task not found" });
            }

            _logger.LogInformation("Task with ID {TaskId} retrieved successfully", id);
            return Ok(result);
        }

        [HttpGet("getStatistics")]
        [Authorize]
        public async Task<IActionResult> GetTaskStatistics()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(new GetTaskStatisticsCommand { UserId = userId });

            if (result == null)
            {
                _logger.LogError("Failed to retrieve task statistics for user {UserId}", userId);
                return BadRequest(new { Message = "Failed to retrieve task statistics" });
            }

            _logger.LogInformation("Task statistics retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskCommand command, [FromRoute] int id)
        {
            command.Id = id;
            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (command.Id <= 0)
            {
                _logger.LogError("Invalid task ID: {TaskId}", command.Id);
                return BadRequest(new { Message = "Invalid task ID" });
            }

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update task with ID {TaskId}", command.Id);
                return BadRequest(new { Message = "Failed to update task" });
            }

            _logger.LogInformation("Task with ID {TaskId} updated successfully", command.Id);
            return Ok(result);

        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid task ID: {TaskId}", id);
                return BadRequest(new { Message = "Invalid task ID" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(new DeleteTaskCommand(id));

            if (result == null)
            {
                _logger.LogError("Failed to delete task with ID {TaskId}", id);
                return BadRequest(new { Message = "Failed to delete task" });
            }

            _logger.LogInformation("Task with ID {TaskId} deleted successfully", id);
            return Ok(result);

        }

        [HttpGet("getTopTasks")]
        [Authorize]
        public async Task<IActionResult> GetTopTasks()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID not found in claims");
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var result = await _mediator.Send(new GetTopTasksCommand(userId));

            if (result == null || !result.Data.Any())
            {
                _logger.LogInformation("No top tasks found for user {UserId}", userId);
                return NotFound(new { Message = "No top tasks found" });
            }

            _logger.LogInformation("Top tasks retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpPost("schedule")]
        [Authorize]
        public async Task<IActionResult> ScheduleTask([FromBody] ScheduleTaskCommand command)
        {
            command.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to schedule task for user {UserId}", command.OwnerId);
                return BadRequest(new { Message = "Failed to schedule task" });
            }

            _logger.LogInformation("Task scheduled successfully for user {UserId}", command.OwnerId);
            return Ok(result);
        }

        
    }
}