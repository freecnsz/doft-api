using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {

            var tag = await _context.Tags
                .Where(t => t.Name == name)
                .FirstOrDefaultAsync();

            return tag;
        }
    }
    
}