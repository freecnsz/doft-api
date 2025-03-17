using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.Interfaces.ServiceInterfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(string userId);
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
        Task RevokeRefreshTokenAsync(string userId);
    }
}