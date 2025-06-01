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
    public class DeleteCategoryCommand : IRequest<ApiResponse<bool>>
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }

        public DeleteCategoryCommand(int id, string userId)
        {
            Id = id;
            UserId = userId;
        }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ILogger<DeleteCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
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
                    return ApiResponse<bool>.Error(403, "You don't have permission to delete this category", false);
                }

                // Delete the user-category relationship
                await _categoryRepository.DeleteUserCategoryAsync(userCategory);

                // If this was the last user for this category, delete the category itself
                var remainingUserCategories = await _categoryRepository.GetUserCategoryAsync(category.Id, null);
                if (remainingUserCategories == null)
                {
                    await _categoryRepository.DeleteAsync(category);
                }

                _logger.LogInformation("Category {CategoryId} deleted successfully for user {UserId}", request.Id, request.UserId);
                return ApiResponse<bool>.Success(200, "Category deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId} for user {UserId}", request.Id, request.UserId);
                return ApiResponse<bool>.Error(500, "An error occurred while deleting the category", false);
            }
        }
    }
} 