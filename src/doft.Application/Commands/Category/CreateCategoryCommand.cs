using System.Text.Json.Serialization;
using doft.Application.DTOs;
using doft.Application.DTOs.Category;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Category
{
    public class CreateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
    {
        [JsonIgnore]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public CreateCategoryCommand(string name, string color)
        {
            Name = name;
            Color = color ?? "#000000";
        }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ILogger<CreateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if category exists
                var existingCategory = await _categoryRepository.GetByNameAsync(request.Name);
                Domain.Entities.Category category;

                if (existingCategory != null)
                {
                    // Check if user already has this category
                    var userCategoryExists = await _categoryRepository.GetUserCategoryAsync(existingCategory.Id, request.UserId);
                    if (userCategoryExists != null)
                    {
                        _logger.LogWarning("Category with name {CategoryName} already exists for user {UserId}", request.Name, request.UserId);
                        return ApiResponse<CategoryDto>.Error(400, "Category with this name already exists for the user", null);
                    }

                    // Category exists but not linked to user, create the relationship
                    category = existingCategory;
                    var userCategory = new UserCategory
                    {
                        UserId = request.UserId,
                        CategoryId = category.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _categoryRepository.AddUserCategoryAsync(userCategory);
                    _logger.LogInformation("Existing category {CategoryName} linked to user {UserId}", request.Name, request.UserId);
                }
                else
                {
                    // Create new category
                    category = new Domain.Entities.Category
                    {
                        Name = request.Name,
                        Color = request.Color,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    category = await _categoryRepository.AddAsync(category);

                    // Create user-category relationship
                    var userCategory = new UserCategory
                    {
                        UserId = request.UserId,
                        CategoryId = category.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _categoryRepository.AddUserCategoryAsync(userCategory);
                    _logger.LogInformation("New category {CategoryName} created and linked to user {UserId}", request.Name, request.UserId);
                }

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Color = category.Color,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return ApiResponse<CategoryDto>.Success(201, "Category created/linked successfully", categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/linking category for user {UserId}", request.UserId);
                return ApiResponse<CategoryDto>.Error(500, "An error occurred while creating/linking the category", null);
            }
        }
    }
} 