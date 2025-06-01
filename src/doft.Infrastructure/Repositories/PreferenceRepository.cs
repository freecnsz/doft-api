using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class PreferenceRepository : GenericRepository<Preference>, IPreferenceRepository
    {
        private readonly ApplicationDbContext _context;
        public PreferenceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Preference> GetByUserIdAsync(string userId)
        {
            var preference = await _context.Preferences
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync();

            return preference ?? throw new InvalidOperationException("Preference not found for the given user ID.");
        }
    }
    
}