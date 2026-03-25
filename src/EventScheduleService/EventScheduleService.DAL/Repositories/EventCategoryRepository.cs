using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class EventCategoryRepository(EventScheduleDbContext context) : IEventCategoryRepository
{
    public async Task<IEnumerable<EventCategory>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.EventCategories
            .Where(ec => ec.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task<EventCategory?> GetByIdAsync(Guid eventId)
    {
        return await context.EventCategories
            .Include(ec => ec.SoloEvents)
            .Include(ec => ec.RegularEvents)
            .FirstOrDefaultAsync(ec => ec.Id == eventId);
    }

    public async Task<EventCategory> AddAsync(Guid spaceId, string title, string description)
    {
        var eventCategoryEntity = new EventCategory
        {
            Id = Guid.NewGuid(),
            SpaceId = spaceId,
            Title = title,
            Description = description
        };
        context.EventCategories.Add(eventCategoryEntity);
        await context.SaveChangesAsync();
        return eventCategoryEntity;
    }

    public async Task<EventCategory?> UpdateAsync(EventCategory updatedEvent)
    {
        var eventCategoryEntity = new EventCategory
        {
            Id = updatedEvent.Id,
            SpaceId = updatedEvent.SpaceId,
            Title = updatedEvent.Title,
            Description = updatedEvent.Description
        };
        
        var existingEventCategory = await context.EventCategories.FindAsync(eventCategoryEntity.Id);
        if (existingEventCategory == null)
        {
            return null;
        }
        
        context.Entry(existingEventCategory)
            .CurrentValues.SetValues(eventCategoryEntity);
        await context.SaveChangesAsync();
        return existingEventCategory;
    }

    public async Task<bool> DeleteAsync(Guid eventId)
    {
        var eventCategoryEntity = await context.EventCategories.FindAsync(eventId);
        if (eventCategoryEntity == null)
        {
            return false;
        }
        context.EventCategories.Remove(eventCategoryEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBySpaceIdAsync(Guid spaceId)
    {
        var eventCategories = await context.EventCategories
            .Where(ec => ec.SpaceId == spaceId)
            .ToListAsync();
        if (eventCategories.Count == 0)
        {
            return false;
        }
        context.EventCategories.RemoveRange(eventCategories);
        await context.SaveChangesAsync();
        return true;
    }
}
