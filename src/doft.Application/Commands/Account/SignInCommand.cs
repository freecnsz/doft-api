using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using doft.Application.DTOs.Account;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Application.Mappers.Account;
using doft.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace doft.Application.Commands.Account
{
    public class SignInCommand : IRequest<ApiResponse<SignInResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignInCommandHandler : IRequestHandler<SignInCommand, ApiResponse<SignInResponseDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<SignInCommandHandler> _logger;

        public SignInCommandHandler(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService,
            ILogger<SignInCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<ApiResponse<SignInResponseDto>> Handle(
            SignInCommand request, 
            CancellationToken cancellationToken)
        {

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {   
                    _logger.LogError("User not found. Email: {Email}", user.Email);
                    return ApiResponse<SignInResponseDto>.Error(400, "User not found.", null);
                }
    
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    _logger.LogError("Invalid password or email. Email: {Email}", user.Email);
                    await _userManager.AccessFailedAsync(user);
                    return ApiResponse<SignInResponseDto>.Error(400, "Invalid password or email.", null);
                }
    
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user.Id, user.Email, roles.FirstOrDefault() ?? "User");
                var response = user.ToSignInResponseDto(token);
                
                _logger.LogInformation("User signed in successfully. Email: {Email}", user.Email);
                return ApiResponse<SignInResponseDto>.Success(200, "User signed in successfully.", response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while signing in. Email: {Email}", request.Email);
                return ApiResponse<SignInResponseDto>.Error(500, e.Message, null);
            }
        }
    }

}