using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Preference
{
    public class DeletePreferenceCommand : IRequest<ApiResponse<bool>>
    {
        public int PreferenceId { get; set; }

        public DeletePreferenceCommand(int preferenceId)
        {
            PreferenceId = preferenceId;
        }
    }
    
    public class DeletePreferenceCommandHandler : IRequestHandler<DeletePreferenceCommand, ApiResponse<bool>>
    {
        private readonly IPreferenceRepository _preferenceRepository;
        private readonly ILogger<DeletePreferenceCommandHandler> _logger;

        public DeletePreferenceCommandHandler(IPreferenceRepository preferenceRepository, ILogger<DeletePreferenceCommandHandler> logger)
        {
            _preferenceRepository = preferenceRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeletePreferenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var preference = await _preferenceRepository.GetByIdAsync(request.PreferenceId);
                if (preference == null)
                {
                    _logger.LogWarning("Preference with ID {PreferenceId} not found.", request.PreferenceId);
                    return ApiResponse<bool>.Error(404, "Preference not found.", false);
                }
                
                await _preferenceRepository.DeleteAsync(preference);

                _logger.LogInformation("Preference with ID {PreferenceId} deleted successfully.", request.PreferenceId);
                return ApiResponse<bool>.Success(200, "Preference deleted successfully.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting preference with ID {PreferenceId}", request.PreferenceId);
                return ApiResponse<bool>.Error(500, "An error occurred while deleting the preference.", false);
            }
        }
    }
}