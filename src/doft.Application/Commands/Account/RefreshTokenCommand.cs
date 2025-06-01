using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Account;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;

namespace doft.Application.Commands.Account
{
    public class RefreshTokenCommand : IRequest<ApiResponse<RefreshTokenResponseDto>>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<RefreshTokenResponseDto>>
    {
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService,
            UserManager<AppUser> userManager,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<RefreshTokenResponseDto>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromToken(request.AccessToken);
                if (principal == null)
                {
                    _logger.LogError("Invalid access token");
                    return ApiResponse<RefreshTokenResponseDto>.Error(400, "Invalid access token", null);
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in token");
                    return ApiResponse<RefreshTokenResponseDto>.Error(400, "Invalid token claims", null);
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError("User not found. UserId: {UserId}", userId);
                    return ApiResponse<RefreshTokenResponseDto>.Error(400, "User not found", null);
                }

                var isValidRefreshToken = await _refreshTokenService.ValidateRefreshTokenAsync(userId, request.RefreshToken);
                if (!isValidRefreshToken)
                {
                    _logger.LogError("Invalid refresh token for user {UserId}", userId);
                    return ApiResponse<RefreshTokenResponseDto>.Error(400, "Invalid refresh token", null);
                }

                // Generate new tokens
                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = _jwtService.GenerateToken(user.Id, user.Email, roles.FirstOrDefault() ?? "User");
                var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

                var response = new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };

                _logger.LogInformation("Tokens refreshed successfully for user {UserId}", userId);
                return ApiResponse<RefreshTokenResponseDto>.Success(200, "Tokens refreshed successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing tokens");
                return ApiResponse<RefreshTokenResponseDto>.Error(500, "Error refreshing tokens", null);
            }
        }
    }
}