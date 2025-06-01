using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.User;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.User
{
    public class GetUserInfoCommand : IRequest<ApiResponse<GetUserInfoResponseDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetUserInfoCommand(string userId)
        {
            UserId = userId;
        }

    }

    public class GetUserInfoCommandHandler : IRequestHandler<GetUserInfoCommand, ApiResponse<GetUserInfoResponseDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<GetUserInfoCommandHandler> _logger;

        public GetUserInfoCommandHandler(UserManager<AppUser> userManager, ILogger<GetUserInfoCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        
        public async Task<ApiResponse<GetUserInfoResponseDto>> Handle(GetUserInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {request.UserId} not found.");
                    return ApiResponse<GetUserInfoResponseDto>.Error(404, "User not found", null);
                }

                var userInfo = new GetUserInfoResponseDto
                {
                    Username = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Bio = user.Bio,
                    IsActive = user.IsActive,
                    UpdatedAt = user.UpdatedAt
                };

                _logger.LogInformation($"User info retrieved successfully for ID {request.UserId}");
                return ApiResponse<GetUserInfoResponseDto>.Success(200, "User info retrieved successfully", userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving user info for ID {request.UserId}");
                return ApiResponse<GetUserInfoResponseDto>.Error(500, "Internal server error", null);
            }
        }
    }
}