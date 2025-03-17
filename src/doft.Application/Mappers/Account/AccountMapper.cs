using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Commands.Account;
using doft.Application.DTOs.Account;
using doft.Domain.Entities;

namespace doft.Application.Mappers.Account
{
    public static class AccountMapper
    {

        public static RegisterResponseDto ToRegisterResponseDto(this AppUser user)
        {
            return new RegisterResponseDto
            {
                Username = user.UserName,
                Email = user.Email,
            };
        }

        public static SignInResponseDto ToSignInResponseDto(this AppUser user, string token, string refreshToken)
        {
            return new SignInResponseDto
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }
       
    }
}