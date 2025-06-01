using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace doft.Infrastructure.Repositories
{
    public class PlannedTaskRepository : GenericRepository<PlannedTask>, IPlannedTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public PlannedTaskRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PlannedTask>> GetPlannedTasksForUserOnDate(string userId, DateTime date)
        {
            // Ensure date is in UTC
            var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            return await _context.PlannedTasks
                .Include(pt => pt.DoftTask)
                .Where(pt => pt.PlannedDate == utcDate && pt.DoftTask.OwnerId == userId)
                .ToListAsync();
        }

        public async Task<bool> Exists(int taskId)
        {
            return await _context.PlannedTasks.AnyAsync(pt => pt.TaskId == taskId);
        }

        public async Task<List<PlannedTask>> GetTopTasksAsync(string userId)
        {
            return await _context.PlannedTasks
                .Where(pt => pt.DoftTask.OwnerId == userId)
                .Include(pt => pt.DoftTask)
                    .ThenInclude(dt => dt.Detail)
                .OrderBy(pt => pt.PlannedDate)
                .ThenBy(pt => pt.StartTime)
                .Take(3)
                .ToListAsync();
        }

    }
}