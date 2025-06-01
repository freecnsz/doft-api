using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<ApiResponse<bool>>
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public UpdateCategoryCommand(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<bool>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the category
                var category = await _categoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
                    return ApiResponse<bool>.NotFound("Category not found");
                }

                // Check if user has access to this category
                var userCategory = await _categoryRepository.GetUserCategoryAsync(category.Id, request.UserId);
                if (userCategory == null)
                {
                    _logger.LogWarning("User {UserId} does not have access to category {CategoryId}", request.UserId, request.Id);
                    return ApiResponse<bool>.Error(403, "You don't have permission to update this category", false);
                }

                // Check if new name already exists for this user
                if (!string.IsNullOrEmpty(request.Name) && request.Name != category.Name)
                {
                    var existingCategory = await _categoryRepository.GetByNameAndUserIdAsync(request.Name, request.UserId);
                    if (existingCategory != null)
                    {
                        _logger.LogWarning("Category with name {Name} already exists for user {UserId}", request.Name, request.UserId);
                        return ApiResponse<bool>.Error(400, "A category with this name already exists", false);
                    }
                }

                // Update category
                category.Name = request.Name ?? category.Name;
                category.Color = request.Color ?? category.Color;
                category.UpdatedAt = DateTime.UtcNow;

                category = await _categoryRepository.UpdateAsync(category);

                _logger.LogInformation("Category {CategoryId} updated successfully for user {UserId}", request.Id, request.UserId);
                return ApiResponse<bool>.Success(200, "Category updated successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId} for user {UserId}", request.Id, request.UserId);
                return ApiResponse<bool>.Error(500, "An error occurred while updating the category", false);
            }
        }
    }
}