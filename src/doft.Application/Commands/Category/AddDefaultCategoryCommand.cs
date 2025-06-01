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
    public class AddDefaultCategoryCommand : IRequest<ApiResponse<bool>>
    {
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public AddDefaultCategoryCommand(string categoryName, string categoryColor)
        {
            CategoryName = categoryName;
            CategoryColor = categoryColor ?? "#000000"; // Default color if not provided
        }

    }
    
    public class AddDefaultCategoryCommandHandler : IRequestHandler<AddDefaultCategoryCommand, ApiResponse<bool>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<AddDefaultCategoryCommandHandler> _logger;
        public async Task<ApiResponse<bool>> Handle(AddDefaultCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = new Domain.Entities.Category
                {
                    Name = request.CategoryName,
                    Color = request.CategoryColor,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Check if the category already exists
                var existingCategory = await _categoryRepository.GetByNameAsync(request.CategoryName);
                if (existingCategory != null)
                {
                    _logger.LogWarning("Default category with name {CategoryName} already exists.", request.CategoryName);
                    return ApiResponse<bool>.Error(400, "Default category already exists", false);
                }

                // Add the new category
                await _categoryRepository.AddAsync(category);
                _logger.LogInformation("Default category {CategoryName} added successfully.", request.CategoryName);
                return ApiResponse<bool>.Success(200, "Default category added successfully", true);

            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                _logger.LogError(ex, "An error occurred while adding default category {CategoryName}", request.CategoryName);
                return ApiResponse<bool>.Error(500, "An error occurred while adding default categories", false);
            }
        }
    }
}