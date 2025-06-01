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
    public class AddPreferenceCommand : IRequest<ApiResponse<AddPreferenceResponseDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        public bool NotificationEnabled { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }

        public AddPreferenceCommand(string userId, string language, string theme, bool notificationEnabled, bool emailNotifications, bool pushNotifications, string timeZone, string dateFormat, string timeFormat)
        {
            UserId = userId;
            Language = language;
            Theme = theme;
            NotificationEnabled = notificationEnabled;
            EmailNotifications = emailNotifications;
            PushNotifications = pushNotifications;
            TimeZone = timeZone;
            DateFormat = dateFormat;
            TimeFormat = timeFormat;
        }
    }
    

    public class AddPreferenceCommandHandler : IRequestHandler<AddPreferenceCommand, ApiResponse<AddPreferenceResponseDto>>
    {
        private readonly IPreferenceRepository _preferenceRepository;
        private readonly ILogger<AddPreferenceCommandHandler> _logger;

        public AddPreferenceCommandHandler(IPreferenceRepository preferenceRepository, ILogger<AddPreferenceCommandHandler> logger)
        {
            _preferenceRepository = preferenceRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<AddPreferenceResponseDto>> Handle(AddPreferenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var preference = new Domain.Entities.Preference
                {
                    UserId = request.UserId,
                    Language = request.Language,
                    Theme = request.Theme,
                    NotificationEnabled = request.NotificationEnabled,
                    EmailNotifications = request.EmailNotifications,
                    PushNotifications = request.PushNotifications,
                    TimeZone = request.TimeZone,
                    DateFormat = request.DateFormat,
                    TimeFormat = request.TimeFormat,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _preferenceRepository.AddAsync(preference);
                if (result == null)
                {
                    return ApiResponse<AddPreferenceResponseDto>.Error(500, "Failed to add preference", null);
                }

                return ApiResponse<AddPreferenceResponseDto>.Success(200, "Preference added successfully", new AddPreferenceResponseDto
                {
                    Id = result.Id,
                    CreatedAt = result.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding preference");
                return ApiResponse<AddPreferenceResponseDto>.Error(500, "Internal server error", null);
            }
        }
    }
}