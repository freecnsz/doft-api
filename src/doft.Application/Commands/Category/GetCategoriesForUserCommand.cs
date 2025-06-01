using System.Text.Json.Serialization;
using doft.Application.DTOs;
using doft.Application.DTOs.Category;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Category
{
    public class GetCategoriesForUserCommand : IRequest<ApiResponse<List<CategoryDto>>>
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public GetCategoriesForUserCommand(string userId)
        {
            UserId = userId;
        }
    }

    public class GetCategoriesForUserCommandHandler : IRequestHandler<GetCategoriesForUserCommand, ApiResponse<List<CategoryDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetCategoriesForUserCommandHandler> _logger;

        public GetCategoriesForUserCommandHandler(
            ICategoryRepository categoryRepository,
            ILogger<GetCategoriesForUserCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CategoryDto>>> Handle(GetCategoriesForUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesForUserAsync(request.UserId);
                
                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} categories for user {UserId}", categoryDtos.Count, request.UserId);
                return ApiResponse<List<CategoryDto>>.Success(200, "Categories retrieved successfully", categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories for user {UserId}", request.UserId);
                return ApiResponse<List<CategoryDto>>.Error(500, "An error occurred while retrieving categories", null);
            }
        }
    }
} 