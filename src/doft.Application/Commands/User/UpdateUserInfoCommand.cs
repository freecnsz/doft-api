using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.User
{ 
    public class UpdateUserInfoCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }

        public UpdateUserInfoCommand(string userId, string fullName, string bio)
        {
            UserId = userId;
            FullName = fullName;
            Bio = bio;
        }
    }

    public class UpdateUserInfoCommandHandler : IRequestHandler<UpdateUserInfoCommand, ApiResponse<bool>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UpdateUserInfoCommandHandler> _logger;

        public UpdateUserInfoCommandHandler(UserManager<AppUser> userManager, ILogger<UpdateUserInfoCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {request.UserId} not found.");
                    return ApiResponse<bool>.Error(404, "User not found", false);
                }

                user.FullName = request.FullName;
                user.Bio = request.Bio;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return ApiResponse<bool>.Success(200, "User info updated successfully", true);
                }
                
                return ApiResponse<bool>.Error(500, "Failed to update user info", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user info");
                return ApiResponse<bool>.Error(500, "Internal server error", false);
            }
        }
    }
}