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

namespace doft.Application.Commands.Account
{
    public class ChangePasswordCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public ChangePasswordCommand(string userId, string currentPassword, string newPassword, string confirmPassword)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmPassword = confirmPassword;
        }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(UserManager<AppUser> userManager, ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogError($"User not found with ID: {request.UserId}");
                    return ApiResponse<bool>.NotFound($"User not found.");
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password changed successfully for user ID: {request.UserId}");
                    return ApiResponse<bool>.Success(200, "Password changed successfully.", true);
                }

                _logger.LogError($"Failed to change password for user ID: {request.UserId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return ApiResponse<bool>.Error(400, "Failed to change password", false);
            }
            catch (System.Exception)
            {
                _logger.LogError($"An error occurred while changing password for user ID: {request.UserId}");
                return ApiResponse<bool>.Error(500, "An error occurred while changing password", false);
            }
        }
    }

}