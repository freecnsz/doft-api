using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.User;
using doft.Application.Interfaces.ServiceInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.User
{
    public class GetProfilePictureCommand : IRequest<ApiResponse<UpdateProfilePictureResponseDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetProfilePictureCommand(string userId)
        {
            UserId = userId;
        }
    }

    public class GetProfilePictureCommandHandler : IRequestHandler<GetProfilePictureCommand, ApiResponse<UpdateProfilePictureResponseDto>>
    {
        private readonly IS3Service _s3Service;
        private readonly ILogger<GetProfilePictureCommandHandler> _logger;

        public GetProfilePictureCommandHandler(IS3Service s3Service, ILogger<GetProfilePictureCommandHandler> logger)
        {
            _logger = logger;
            _s3Service = s3Service;
        }

        public async Task<ApiResponse<UpdateProfilePictureResponseDto>> Handle(GetProfilePictureCommand request, CancellationToken cancellationToken)
        {
            var profilePictureUrl = await _s3Service.GetProfilePictureUrlAsync(request.UserId);
            if (string.IsNullOrEmpty(profilePictureUrl))
            {
                _logger.LogError("Profile picture not found for user {UserId}", request.UserId);
                return ApiResponse<UpdateProfilePictureResponseDto>.Error(404, "Profile picture not found", null);
            }

            var response = new UpdateProfilePictureResponseDto
            {
                ProfilePictureUrl = profilePictureUrl
            };

            _logger.LogInformation("Profile picture retrieved successfully for user {UserId}", request.UserId);
            return ApiResponse<UpdateProfilePictureResponseDto>.Success(200, "Profile picture retrieved successfully", response);
        }
    }
    
        
    
}