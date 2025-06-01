using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface IPlannedTaskRepository : IGenericRepository<PlannedTask>
    {
        Task<List<PlannedTask>> GetPlannedTasksForUserOnDate(string userId, DateTime date);
        Task<bool> Exists(int taskId);
        Task<List<PlannedTask>> GetTopTasksAsync(string userId);
    }
}