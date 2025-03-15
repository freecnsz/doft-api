using doft.Application.DTOs;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace doft.Webapi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                // Log the validation exception
                _logger.LogError(ex, "Validation error occurred.");

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error(400, "Validation error occurred.", ex.Errors.Select(e => e.ErrorMessage).ToList());

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                // Log the general exception
                _logger.LogError(ex, "An unexpected error occurred.");

                await HandleGeneralExceptionAsync(context, ex);
            }
        }

        private static Task HandleGeneralExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Error(500, "An unexpected error occurred.", exception.Message);

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
