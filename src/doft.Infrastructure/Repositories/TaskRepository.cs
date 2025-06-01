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
    public class TaskRepository : GenericRepository<DoftTask>, ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DoftTask>> GetAllTasksForUserAsync(string userId)
        {
            var tasks = await _context.DoftTasks
                .Include(t => t.Category)
                .Include(t => t.Detail)
                .Include(t => t.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .Where(t => t.OwnerId == userId &&
                           !t.Detail.IsDeleted &&
                           (t.Status == DoftTaskStatus.Pending || t.Status == DoftTaskStatus.InProgress) &&
                           t.DueDate >= DateTime.UtcNow)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.TagLinks = task.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Task)
                    .ToList();
            }

            return tasks;
        }

        public override async Task<DoftTask?> GetByIdAsync(int id)
        {
            var task = await _context.DoftTasks
                .Include(t => t.Detail)
                .Include(t => t.Category)
                .Include(t => t.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task != null)
            {
                task.TagLinks = task.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Task)
                    .ToList();
            }

            return task;
        }

        public async Task<List<DoftTask>> GetCompletedTasksForUser(string userId, CancellationToken cancellationToken)
        {
            return await _context.DoftTasks
                .Where(t => t.OwnerId == userId &&
                            !t.Detail.IsDeleted &&
                            t.Status == DoftTaskStatus.Completed)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DoftTask>> GetRemainingTasksForUser(string userId, CancellationToken cancellationToken)
        {
            return await _context.DoftTasks
                .Where(t => t.OwnerId == userId &&
                            !t.Detail.IsDeleted &&
                            (t.Status == DoftTaskStatus.Pending || t.Status == DoftTaskStatus.InProgress) &&
                            t.DueDate >= DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DoftTask>> GetTasksByCategory(int categoryId, string userId)
        {
            var tasks = await _context.DoftTasks
                .Include(t => t.Detail)
                .Include(t => t.Category)
                .Include(t => t.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .Where(t => t.CategoryId == categoryId &&
                            t.OwnerId == userId &&
                            !t.Detail.IsDeleted &&
                            (t.Status == DoftTaskStatus.Pending || t.Status == DoftTaskStatus.InProgress) &&
                            t.DueDate >= DateTime.UtcNow)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.TagLinks = task.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Task)
                    .ToList();
            }

            return tasks;
        }

        public async Task<List<DoftTask>> GetTasksByFilters(int? year, int? month, int? day, int? categoryId, string userId)
        {
            var query = _context.DoftTasks
                .Include(t => t.Detail)
                .Include(t => t.Category)
                .Include(t => t.TagLinks)
                    .ThenInclude(tl => tl.Tag)
                .Where(t => t.OwnerId == userId &&
                            !t.Detail.IsDeleted &&
                            (t.Status == DoftTaskStatus.Pending || t.Status == DoftTaskStatus.InProgress) &&
                            t.DueDate >= DateTime.UtcNow);

            if (year.HasValue)
                query = query.Where(t => t.DueDate.Year == year.Value);

            if (month.HasValue)
                query = query.Where(t => t.DueDate.Month == month.Value);

            if (day.HasValue)
                query = query.Where(t => t.DueDate.Day == day.Value);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(t => t.CategoryId == categoryId);

            var tasks = await query.ToListAsync();

            foreach (var task in tasks)
            {
                task.TagLinks = task.TagLinks
                    .Where(tl => tl.ItemType == ItemType.Task)
                    .ToList();
            }

            return tasks;
        }
    }
}
