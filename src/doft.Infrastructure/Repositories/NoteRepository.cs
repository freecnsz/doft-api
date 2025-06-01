using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class NoteRepository(ApplicationDbContext context) : GenericRepository<Note>(context), INoteRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Note>> GetAllNotesForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var notes = await _context.Notes
                .Include(n => n.Detail)
                .Include(n => n.Category)
                .Include(n => n.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .Where(n => n.OwnerId == userId && n.Detail.IsDeleted == false)
                .ToListAsync(cancellationToken);

            foreach (var note in notes)
            {
                note.TagLinks = note.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Note)
                    .ToList();
            }

            return notes;
        }

        public override async Task<Note?> GetByIdAsync(int id)
        {
            var note = await _context.Notes
                .Include(n => n.Detail)
                .Include(n => n.Category)
                .Include(n => n.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .FirstOrDefaultAsync(n => n.Id == id && n.Detail.IsDeleted == false);

            if (note != null)
            {
                note.TagLinks = note.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Note)
                    .ToList();
            }

            return note;
        }

        public async Task<List<Note>> GetNotesByCategoryAsync(int categoryId, string userId, CancellationToken cancellationToken)
        {
            var notes = await _context.Notes
                .Include(n => n.Detail)
                .Include(n => n.Category)
                .Include(n => n.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .Where(n => n.OwnerId == userId && n.CategoryId == categoryId && n.Detail.IsDeleted == false)
                .ToListAsync(cancellationToken);

            foreach (var note in notes)
            {
                note.TagLinks = note.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Note)
                    .ToList();
            }

            return notes;
        }
    }
}
