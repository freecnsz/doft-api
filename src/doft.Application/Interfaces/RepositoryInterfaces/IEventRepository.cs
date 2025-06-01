using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<List<Event>> GetAllEventsForUserAsync(string userId);
        Task<List<Event>> GetEventsByCategory(int categoryId, string userId);
        Task<List<Event>> GetEventsByFilters(int? year, int? month, int? day, int? categoryId, string userId);
    }
}