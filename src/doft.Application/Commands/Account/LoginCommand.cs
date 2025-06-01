using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Account;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Application.Mappers.Account;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace doft.Application.Commands.Account
{
    public class LoginCommand : IRequest<ApiResponse<LoginResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IRefreshTokenService refreshTokenService;

        public LoginCommandHandler(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService,
            ILogger<LoginCommandHandler> logger,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
            this.refreshTokenService = refreshTokenService;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(
            LoginCommand request, 
            CancellationToken cancellationToken)
        {

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {   
                    _logger.LogError("User not found. Email: {Email}", request.Email);
                    return ApiResponse<LoginResponseDto>.Error(400, "User not found.", null);
                }
    
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    _logger.LogError("Invalid password or email. Email: {Email}", request.Email);
                    await _userManager.AccessFailedAsync(user);
                    return ApiResponse<LoginResponseDto>.Error(400, "Invalid password or email.", null);
                }
    
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user.Id, user.Email, roles.FirstOrDefault() ?? "User");
                var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);
                var response = user.ToSignInResponseDto(token, refreshToken);
                
                _logger.LogInformation("User signed in successfully. Id: {Id}, Email: {Email}", user.Id, user.Email);
                return ApiResponse<LoginResponseDto>.Success(200, "User signed in successfully.", response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while signing in. Email: {Email}", request.Email);
                return ApiResponse<LoginResponseDto>.Error(500, e.Message, null);
            }
        }

    }

}