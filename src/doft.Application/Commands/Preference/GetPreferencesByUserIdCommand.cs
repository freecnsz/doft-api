using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Preferences;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Preference
{
    public class GetPreferencesByUserIdCommand : IRequest<ApiResponse<GetPreferencesByUserIdResponseDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetPreferencesByUserIdCommand(string userId)
        {
            UserId = userId;
        }
    }
    
    public class GetPreferencesByUserIdCommandHandler : IRequestHandler<GetPreferencesByUserIdCommand, ApiResponse<GetPreferencesByUserIdResponseDto>>
    {
        private readonly IPreferenceRepository _preferenceRepository;
        private readonly ILogger<GetPreferencesByUserIdCommandHandler> _logger;

        public GetPreferencesByUserIdCommandHandler(IPreferenceRepository preferenceRepository, ILogger<GetPreferencesByUserIdCommandHandler> logger)
        {
            _logger = logger;
            _preferenceRepository = preferenceRepository;
        }

        public async Task<ApiResponse<GetPreferencesByUserIdResponseDto>> Handle(GetPreferencesByUserIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var preferences = await _preferenceRepository.GetByUserIdAsync(request.UserId);
                if (preferences == null)
                {
                    _logger.LogWarning("No preferences found for user {UserId}", request.UserId);
                    return ApiResponse<GetPreferencesByUserIdResponseDto>.Error(404, "No preferences found for the given user ID.", null);
                }

                var response = new GetPreferencesByUserIdResponseDto
                {
                    Id = preferences.Id,
                    UserId = preferences.UserId,
                    Language = preferences.Language,
                    Theme = preferences.Theme,
                    NotificationEnabled = preferences.NotificationEnabled,
                    EmailNotifications = preferences.EmailNotifications,
                    PushNotifications = preferences.PushNotifications,
                    TimeZone = preferences.TimeZone,
                    DateFormat = preferences.DateFormat,
                    TimeFormat = preferences.TimeFormat,
                    CreatedAt = preferences.CreatedAt,
                    UpdatedAt = preferences.UpdatedAt
                };

                _logger.LogInformation("Preferences retrieved successfully for user {UserId}", request.UserId);
                return ApiResponse<GetPreferencesByUserIdResponseDto>.Success(200, "Preferences retrieved successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving preferences for user {UserId}", request.UserId);
                return ApiResponse<GetPreferencesByUserIdResponseDto>.Error(500, "An error occurred while retrieving preferences", null);
            }
        }
    }
}