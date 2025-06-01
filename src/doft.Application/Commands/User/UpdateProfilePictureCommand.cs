using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.User;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.User
{
    public class UpdateProfilePictureCommand : IRequest<ApiResponse<UpdateProfilePictureResponseDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public IFormFile ProfilePicture { get; set; }

    }

    public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, ApiResponse<UpdateProfilePictureResponseDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IS3Service _s3Service;
        private readonly ILogger<UpdateProfilePictureCommandHandler> _logger;

        public UpdateProfilePictureCommandHandler(UserManager<AppUser> userManager, IS3Service s3Service, ILogger<UpdateProfilePictureCommandHandler> logger)
        {
            _userManager = userManager;
            _s3Service = s3Service;
            _logger = logger;
        }

        public async Task<ApiResponse<UpdateProfilePictureResponseDto>> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _userManager.FindByIdAsync(request.UserId).Result;
                if (user == null)
                {
                    _logger.LogError("User not found with ID {UserId}", request.UserId);
                    return ApiResponse<UpdateProfilePictureResponseDto>.Error(404, "User not found", null);
                }

                var fileName = request.UserId;
                var filePath = await _s3Service.UploadProfilePictureAsync(request.UserId, request.ProfilePicture.OpenReadStream());

                user.ProfilePictureUrl = filePath;
                var result = _userManager.UpdateAsync(user).Result;

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user profile picture for user {UserId}", request.UserId);
                    return ApiResponse<UpdateProfilePictureResponseDto>.Error(500, "Failed to update user profile picture", null);
                }

                var response = new UpdateProfilePictureResponseDto
                {
                    ProfilePictureUrl = filePath
                };

                _logger.LogInformation("Profile picture updated successfully for user {UserId}", request.UserId);
                return ApiResponse<UpdateProfilePictureResponseDto>.Success(200, "Profile picture updated successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile picture");
                return ApiResponse<UpdateProfilePictureResponseDto>.Error(500, "An error occurred while updating the profile picture", null);
            }
        }
    }
    
    
}