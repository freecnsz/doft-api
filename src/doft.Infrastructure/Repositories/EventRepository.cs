using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;
using doft.Domain.Enums;
using doft.Infrastructure;
using doft.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllEventsForUserAsync(string userId)
    {
        var events = await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Detail)
            .Include(e => e.TagLinks)
                .ThenInclude(tl => tl.Tag)
            .Where(e => e.OwnerId == userId && !e.Detail.IsDeleted)
            .Where(e => e.FromDate >= DateTime.UtcNow || e.ToDate >= DateTime.UtcNow)
            .ToListAsync();

        foreach (var e in events)
        {
            e.TagLinks = e.TagLinks
                .Where(tl => tl.ItemType == ItemType.Event)
                .ToList();
        }

        return events;
    }

    public override async Task<Event?> GetByIdAsync(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.Category)
            .Include(e => e.Detail)
            .Include(e => e.TagLinks)
                .ThenInclude(tl => tl.Tag)
            .FirstOrDefaultAsync(e => e.Id == id && !e.Detail.IsDeleted);

        if (eventItem != null)
        {
            eventItem.TagLinks = eventItem.TagLinks
                .Where(tl => tl.ItemType == ItemType.Event)
                .ToList();
        }

        return eventItem;
    }

    public async Task<List<Event>> GetEventsByCategory(int categoryId, string userId)
    {
        var events = await _context.Events
            .Include(e => e.Detail)
            .Include(e => e.Category)
            .Include(e => e.TagLinks)
                .ThenInclude(tl => tl.Tag)
            .Where(e => e.CategoryId == categoryId && e.OwnerId == userId && !e.Detail.IsDeleted)
            .Where(e => e.FromDate >= DateTime.UtcNow || e.ToDate >= DateTime.UtcNow)
            .ToListAsync();

        foreach (var e in events)
        {
            e.TagLinks = e.TagLinks
                .Where(tl => tl.ItemType == ItemType.Event)
                .ToList();
        }

        return events;
    }

    public async Task<List<Event>> GetEventsByFilters(int? year, int? month, int? day, int? categoryId, string userId)
    {
        var query = _context.Events
            .Include(e => e.Detail)
            .Include(e => e.Category)
            .Include(e => e.TagLinks)
                .ThenInclude(tl => tl.Tag)
            .Where(e => e.OwnerId == userId && !e.Detail.IsDeleted);

        if (year.HasValue)
        {
            query = query.Where(e => e.FromDate.Year == year.Value || e.ToDate.Year == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(e => e.FromDate.Month == month.Value || e.ToDate.Month == month.Value);
        }

        if (day.HasValue)
        {
            query = query.Where(e => e.FromDate.Day == day.Value || e.ToDate.Day == day.Value);
        }

        if (categoryId.HasValue && categoryId > 0)
        {
            query = query.Where(e => e.CategoryId == categoryId);
        }

        var events = await query.ToListAsync();

        foreach (var e in events)
        {
            e.TagLinks = e.TagLinks
                .Where(tl => tl.ItemType == ItemType.Event)
                .ToList();
        }

        return events;
    }
}
