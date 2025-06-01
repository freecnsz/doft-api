using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class ReminderRepository : GenericRepository<Reminder>, IReminderRepository
    {
        private readonly ApplicationDbContext _context;
        public ReminderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Reminder>> GetReminderByTaskIdAsync(int taskId)
        {
            return await _context.Reminders
                .Include(r => r.DoftTask)
                .ThenInclude(t => t.Detail)
                .Where(r => r.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(string userId)
        {
            return await _context.Reminders
                .Include(r => r.DoftTask)
                .ThenInclude(t => t.Detail)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}