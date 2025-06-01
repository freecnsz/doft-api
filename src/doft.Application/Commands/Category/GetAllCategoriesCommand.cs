using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Category
{
    public class GetAllCategoriesCommand : IRequest<ApiResponse<List<GetAllCategoiesResponseDto>>>
    {
        
        
        
    }

    public class GetAllCategoriesCommandHandler : IRequestHandler<GetAllCategoriesCommand, ApiResponse<List<GetAllCategoiesResponseDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetAllCategoriesCommandHandler> _logger;

        public GetAllCategoriesCommandHandler(ICategoryRepository categoryRepository, ILogger<GetAllCategoriesCommandHandler> logger)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }
       

        public async Task<ApiResponse<List<GetAllCategoiesResponseDto>>> Handle(GetAllCategoriesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var response = categories.Select(c => new GetAllCategoiesResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                }).ToList();

                _logger.LogInformation("Successfully retrieved all categories.");
                return ApiResponse<List<GetAllCategoiesResponseDto>>.Success(200, "Categories retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                return ApiResponse<List<GetAllCategoiesResponseDto>>.Error(500, "An error occurred while retrieving categories.", null);
            }
        }
    }
}