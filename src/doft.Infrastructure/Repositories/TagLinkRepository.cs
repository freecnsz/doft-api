using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class TagLinkRepository : GenericRepository<TagLink>, ITagLinkRepository
    {
        private readonly ApplicationDbContext _context;
        public TagLinkRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TagLink>> GetAllItemsByTagAsync(int tagId)
        {
            return await _context.TagLinks
                .Include(tl => tl.Tag)
                .Where(tl => tl.TagId == tagId)
                .ToListAsync();
        }

        public async Task<List<TagLink>> GetTagLinksByItemAsync(ItemType itemType, int itemId)
        {
            return await _context.TagLinks
                .Include(tl => tl.Tag)
                .Where(tl => tl.ItemType == itemType && tl.ItemId == itemId)
                .ToListAsync();
        }
    }
}