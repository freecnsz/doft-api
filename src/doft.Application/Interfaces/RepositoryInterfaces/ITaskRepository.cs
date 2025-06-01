using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface ITaskRepository : IGenericRepository<DoftTask>
    {
        Task<List<DoftTask>> GetAllTasksForUserAsync(string userId);
        Task<List<DoftTask>> GetTasksByCategory(int categoryId, string userId);
        Task<List<DoftTask>> GetTasksByFilters(int? year, int? month, int? day, int? categoryId, string userId);
        Task<List<DoftTask>> GetCompletedTasksForUser(string userId, CancellationToken cancellationToken);
        Task<List<DoftTask>> GetRemainingTasksForUser(string userId, CancellationToken cancellationToken);
    }
}