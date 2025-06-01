using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Preference
{
    public class UpdatePreferenceCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        public bool NotificationEnabled { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UpdatePreferenceCommand(int id, string language, string theme, bool notificationEnabled, bool emailNotifications, bool pushNotifications, string timeZone, string dateFormat, string timeFormat, DateTime updatedAt)
        {
            Id = id;
            Language = language;
            Theme = theme;
            NotificationEnabled = notificationEnabled;
            EmailNotifications = emailNotifications;
            PushNotifications = pushNotifications;
            TimeZone = timeZone;
            DateFormat = dateFormat;
            TimeFormat = timeFormat;
            UpdatedAt = updatedAt;
        }
    }

    public class UpdatePreferenceCommandHandler : IRequestHandler<UpdatePreferenceCommand, ApiResponse<bool>>
    {
        private readonly IPreferenceRepository _preferenceRepository;
        private readonly ILogger<UpdatePreferenceCommandHandler> _logger;

        public UpdatePreferenceCommandHandler(IPreferenceRepository preferenceRepository, ILogger<UpdatePreferenceCommandHandler> logger)
        {
            _logger = logger;
            _preferenceRepository = preferenceRepository;
        }

        public async Task<ApiResponse<bool>> Handle(UpdatePreferenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var preference = await _preferenceRepository.GetByIdAsync(request.Id);
                if (preference == null)
                {
                    _logger.LogWarning($"Preference with ID {request.Id} not found.");
                    return ApiResponse<bool>.Error(404, "Preference not found.", false);
                }

                preference.Language = request.Language;
                preference.Theme = request.Theme;
                preference.NotificationEnabled = request.NotificationEnabled;
                preference.EmailNotifications = request.EmailNotifications;
                preference.PushNotifications = request.PushNotifications;
                preference.TimeZone = request.TimeZone;
                preference.DateFormat = request.DateFormat;
                preference.TimeFormat = request.TimeFormat;
                preference.UpdatedAt = DateTime.UtcNow;

                await _preferenceRepository.UpdateAsync(preference);

                _logger.LogInformation($"Preference with ID {request.Id} updated successfully.");
                return ApiResponse<bool>.Success(200, "Preference updated successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preference");
                return ApiResponse<bool>.Error(500, "An error occurred while updating the preference.", false);
            }
        }
    }
    
}