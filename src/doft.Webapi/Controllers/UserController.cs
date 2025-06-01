using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("updateProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureCommand command)
        {
            command.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update profile picture for user {UserId}", command.UserId);
                return BadRequest(new { Message = "Failed to update profile picture" });
            }

            _logger.LogInformation("Profile picture updated successfully for user {UserId}", command.UserId);
            return Ok(result);
        }

        [HttpGet("getProfilePicture")]
        [Authorize]
        public async Task<IActionResult> GetProfilePicture()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetProfilePictureCommand(userId));

            if (result == null)
            {
                _logger.LogError("No profile picture found for user {UserId}", userId);
                return NotFound(new { Message = "No profile picture found for the user" });
            }

            _logger.LogInformation("Profile picture retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpGet("getUserInfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetUserInfoCommand(userId));

            if (result == null)
            {
                _logger.LogError("Failed to retrieve user info for user {UserId}", userId);
                return NotFound(new { Message = "User not found" });
            }

            _logger.LogInformation("User info retrieved successfully for user {UserId}", userId);
            return Ok(result);
        }

        [HttpPut("updateUserInfo")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoCommand command)
        {
            command.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update user info for user {UserId}", command.UserId);
                return BadRequest(new { Message = "Failed to update user info" });
            }

            _logger.LogInformation("User info updated successfully for user {UserId}", command.UserId);
            return Ok(result);
        }
    }

}