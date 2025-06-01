using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Category;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("addDefaultCategory")]
        [Authorize]
        public async Task<IActionResult> AddDefaultCategory([FromBody] AddDefaultCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to add default category.");
                return BadRequest(new { Message = "Failed to add default category." });
            }

            _logger.LogInformation("Default category added successfully.");
            return Ok(result);
        }

        [HttpGet("getAllCategories")]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _mediator.Send(new GetAllCategoriesCommand());

            if (result == null)
            {
                _logger.LogError("No categories found.");
                return NotFound(new { Message = "No categories found." });
            }

            _logger.LogInformation("Successfully retrieved all categories.");
            return Ok(result);
        }

        [HttpPost("addCategory")]
        [Authorize]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = userId;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to add category.");
                return BadRequest(new { Message = "Failed to add category." });
            }

            _logger.LogInformation("Category added successfully.");
            return Ok(result);
        }

        [HttpGet("getAllCategoiresForUser")]
        [Authorize]
        public async Task<IActionResult> GetAllCategoiresForUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetCategoriesForUserCommand(userId));

            if (result == null)
            {
                _logger.LogError("No categories found for user.");
                return NotFound(new { Message = "No categories found for user." });
            }

            _logger.LogInformation("Successfully retrieved all categories for user.");
            return Ok(result);
        }

        [HttpPut("updateCategory/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommand command, [FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.Id = id;
            command.UserId = userId;

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to update category.");
                return BadRequest(new { Message = "Failed to update category." });
            }

            _logger.LogInformation("Category updated successfully.");
            return Ok(result);
        }

        [HttpDelete("deleteCategory/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new DeleteCategoryCommand(id, userId);

            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogError("Failed to delete category.");
                return BadRequest(new { Message = "Failed to delete category." });
            }

            _logger.LogInformation("Category deleted successfully.");
            return Ok(result);

        }
    }
}