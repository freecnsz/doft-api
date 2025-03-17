using System.Security.Cryptography;
using System.Text;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenRepository tokenRepository;

        public RefreshTokenService(ITokenRepository tokenRepository)
        {
            this.tokenRepository = tokenRepository;
        }

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hashedToken = HashToken(refreshToken);

            var existingToken = await tokenRepository.GetByUserIdAsync(userId);
            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                existingToken.PreviousToken = existingToken.Token; // Save old token reference
                await tokenRepository.UpdateAsync(existingToken);
            }

            var newToken = new RefreshToken
            {
                UserId = userId,
                Token = hashedToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                PreviousToken = existingToken?.Token // Save old token reference
            };

            await tokenRepository.AddAsync(newToken);

            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
        {
            var hashedToken = HashToken(refreshToken);
            var storedToken = await tokenRepository.GetByUserIdAsync(userId);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow || storedToken.IsRevoked)
                return false;

            // Prevent token reuse attack
            if (storedToken.PreviousToken == hashedToken)
                return false;

            return storedToken.Token == hashedToken;
        }


        public async Task RevokeRefreshTokenAsync(string userId)
        {
            var tokens = await tokenRepository.GetAllByUserIdAsync(userId);

            if (tokens == null || !tokens.Any()) return;

            foreach (var token in tokens)
            {
                if (!token.IsRevoked) // Only revoke active tokens
                {
                    token.IsRevoked = true;
                }
            }

            await tokenRepository.UpdateRangeAsync(tokens);
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(token)));
        }

    }
}
