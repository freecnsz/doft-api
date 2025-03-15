using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using doft.Application.DTOs.Account;

namespace doft.Application.Interfaces.ServiceInterfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, string role);
        bool ValidateToken(string token);

    }
}