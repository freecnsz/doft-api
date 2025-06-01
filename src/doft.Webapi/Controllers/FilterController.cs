using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.Commands.Filter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FilterController> _logger;

        public FilterController(IMediator mediator, ILogger<FilterController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("getItemsByTag/{tagName}")]
        [Authorize]
        public async Task<IActionResult> GetItemsByTag([FromRoute] string tagName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetByTagCommand(tagName, userId));

            if (result == null)
            {
                _logger.LogError("No items found for user {UserId} with tag {TagName}", userId, tagName);
                return NotFound(new { Message = "No items found for the specified tag" });
            }

            _logger.LogInformation("Items retrieved successfully for user {UserId} with tag {TagName}", userId, tagName);
            return Ok(result);
        }

        [HttpGet("getItemsByCategory/{categoryId}")]
        [Authorize]
        public async Task<IActionResult> GetItemsByCategory([FromRoute] int categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetByCategoryCommand(categoryId, userId));

            if (result == null)
            {
                _logger.LogError("No items found for user {UserId} with category {categoryId}", userId, categoryId);
                return NotFound(new { Message = "No items found for the specified category" });
            }

            _logger.LogInformation("Items retrieved successfully for user {UserId} with category {categoryId}", userId, categoryId);
            return Ok(result);
        }

        [HttpGet("getItemsByFilters")]
        [Authorize]
        public async Task<IActionResult> GetItemsByFilter([FromQuery] int? year, [FromQuery] int? month, [FromQuery] int? day, [FromQuery] int? categoryId, [FromQuery] string? itemType)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _mediator.Send(new GetByFiltersCommand(year, month, day, categoryId, itemType, userId));

            if (result == null)
            {
                _logger.LogError("No items found for user {UserId} on date {Year}-{Month}-{Day}", userId, year, month, day);
                return NotFound(new { Message = "No items found for the specified date" });
            }

            _logger.LogInformation("Items retrieved successfully for user {UserId} on date {Year}-{Month}-{Day}", userId, year, month, day);
            return Ok(result);
        }
       
        
    }
}