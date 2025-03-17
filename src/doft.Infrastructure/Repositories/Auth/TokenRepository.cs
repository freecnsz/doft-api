using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories.Auth
{
    public class TokenRepository : GenericRepository<RefreshToken>, ITokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TokenRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task DeleteRangeAsync(IEnumerable<RefreshToken> tokens)
        {
            _dbContext.RefreshTokens.RemoveRange(tokens);  // Remove multiple refresh tokens
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(string userId)
        {
            return await _dbContext.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();  // Get all tokens for the user
        }

        public async Task<RefreshToken?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsRevoked);
        }

        public async Task UpdateRangeAsync(IEnumerable<RefreshToken> tokens)
        {
            _dbContext.RefreshTokens.UpdateRange(tokens);  // Update multiple refresh tokens at once
            await _dbContext.SaveChangesAsync();  // Commit changes to DB
        }
    }
}