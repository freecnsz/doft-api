using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using doft.Application.Commands.Account;
using doft.Application.Commands.Category;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand model)
        {
            var result = await _mediator.Send(model);

            if (result == null)
            {
                _logger.LogError("User creation failed for {Email}", model.Email);
                return BadRequest(new { Message = "User creation failed" });
            }

            _logger.LogInformation("User created successfully with email {Email}", model.Email);
            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand model)
        {
            var result = await _mediator.Send(model);

            if (result == null)
            {
                _logger.LogError("Sign in failed for {Email}", model.Email);
                return BadRequest(new { Message = "Sign in failed" });
            }

            _logger.LogInformation("User signed in successfully with email {Email}", model.Email);
            return Ok(result);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand model)
        {
            var result = await _mediator.Send(model);

            if (result == null)
            {
                _logger.LogError("Token refresh failed for {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(new { Message = "Token refresh failed" });
            }

            _logger.LogInformation("Token refreshed successfully for {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var result = await _mediator.Send(new LogoutCommand(token));

            if (result == null)
            {
                _logger.LogError("Logout failed for {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(new { Message = "Logout failed" });
            }

            _logger.LogInformation("User logged out successfully with ID {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(result);
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            model.UserId = userId;
            var result = await _mediator.Send(model);

            if (result == null)
            {
                _logger.LogError("Password change failed for user ID {UserId}", userId);
                return BadRequest(new { Message = "Password change failed" });
            }

            _logger.LogInformation("Password changed successfully for user ID {UserId}", userId);
            return Ok(result);
        }

    }
}