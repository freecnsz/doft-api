using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface INoteRepository : IGenericRepository<Note>
    {
        Task<List<Note>> GetAllNotesForUserAsync(string userId, CancellationToken cancellationToken);
        Task<List<Note>> GetNotesByCategoryAsync(int categoryId, string userId, CancellationToken cancellationToken);
    }
}