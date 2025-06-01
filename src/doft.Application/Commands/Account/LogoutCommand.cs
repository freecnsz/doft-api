using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.ServiceInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using doft.Domain.Entities;

namespace doft.Application.Commands.Account
{
    public class LogoutCommand : IRequest<ApiResponse<bool>>
    {
        public string Token { get; set; }

        public LogoutCommand(string token)
        {
            Token = token;
        }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            UserManager<AppUser> userManager,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService,
            ILogger<LogoutCommandHandler> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the access token
                var principal = _jwtService.GetPrincipalFromToken(request.Token);
                if (principal == null)
                {
                    _logger.LogWarning("Invalid access token provided for logout");
                    return ApiResponse<bool>.Error(400, "Invalid access token", false);
                }

                // Get user ID from token
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token");
                    return ApiResponse<bool>.Error(400, "Invalid token claims", false);
                }

                // Verify user exists
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                    return ApiResponse<bool>.Error(404, "User not found", false);
                }

                // Revoke all refresh tokens for the user
                await _refreshTokenService.RevokeRefreshTokenAsync(userId);

                _logger.LogInformation("User {UserId} logged out successfully", userId);
                return ApiResponse<bool>.Success(200, "Logged out successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                return ApiResponse<bool>.Error(500, "An error occurred during logout", false);
            }
        }
    }
}