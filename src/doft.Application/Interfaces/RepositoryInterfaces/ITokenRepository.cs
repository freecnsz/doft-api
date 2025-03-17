using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface ITokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> GetByUserIdAsync(string userId);  // Get the latest refresh token for a user
        Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(string userId);  // Get all refresh tokens for a user
        Task UpdateRangeAsync(IEnumerable<RefreshToken> tokens);  // Update multiple refresh tokens
        Task DeleteRangeAsync(IEnumerable<RefreshToken> tokens);  // Delete multiple refresh tokens
    }
}