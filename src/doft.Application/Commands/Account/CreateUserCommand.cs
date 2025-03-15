
using doft.Application.DTOs;
using doft.Application.DTOs.Account;
using doft.Application.Mappers.Account;
using doft.Domain.Entities;
using doft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace doft.Application.Commands.Account
{
    public class CreateUserCommand : IRequest<ApiResponse<RegisterResponseDto>>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<RegisterResponseDto>>

    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppUserRole> _roleManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            UserManager<AppUser> userManager, 
            RoleManager<AppUserRole> roleManager,
            ILogger<CreateUserCommandHandler> logger
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task<ApiResponse<RegisterResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user already exists
                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if (userExists != null)
                {
                    _logger.LogError($"User already exists with email: {request.Email}");
                    return ApiResponse<RegisterResponseDto>.Error(400, "User already exists", null);
                }



                // Create the user
                var user = new AppUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"User creation failed: {errors}");
                    return ApiResponse<RegisterResponseDto>.Error(400, $"User creation failed: {errors}", null);
                }

                var roleExists = await _roleManager.RoleExistsAsync(request.Role);
                if (!roleExists)
                {
                    _logger.LogError($"Role does not exist: {request.Role}");
                    return ApiResponse<RegisterResponseDto>.Error(400, "Role does not exist", null);
                }

                // Assign role to the user
                var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
                if (!roleResult.Succeeded)
                {
                    // Rollback user creation if role assignment fails
                    await _userManager.DeleteAsync(user);
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"User role assignment failed: {errors}");
                    return ApiResponse<RegisterResponseDto>.Error(400, $"User role assignment failed: {errors}", null);
                }

                _logger.LogInformation($"User created successfully: {user.UserName}");

                // Return success response
                return ApiResponse<RegisterResponseDto>.Success(200, "User created successfully", new RegisterResponseDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                });
            }
            catch (Exception ex)
            {
                var errors = string.Join(", ", ex.Message);
                _logger.LogError($"An error occurred: {errors}");
                return ApiResponse<RegisterResponseDto>.Error(500, $"An error occurred: {errors}", null);
            }
        }

    }
}
