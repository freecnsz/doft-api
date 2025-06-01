using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface IReminderRepository : IGenericRepository<Reminder>
    {
        Task<List<Reminder>> GetRemindersByUserIdAsync(string userId);
        Task<List<Reminder>> GetReminderByTaskIdAsync(int taskId);
        
    }
}