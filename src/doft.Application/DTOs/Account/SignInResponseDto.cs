using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Account
{
    public class SignInResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}