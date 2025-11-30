using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class EventCategoryRepository(EventScheduleDbContext context) : IEventCategoryRepository
{
    public async Task<IEnumerable<IEventCategory>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.EventCategories
            .Where(ec => ec.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task<IEventCategory?> GetByIdAsync(Guid eventId)
    {
        return await context.EventCategories
            .Include(ec => ec.SoloEvents)
            .Include(ec => ec.RegularEvents)
            .FirstOrDefaultAsync(ec => ec.Id == eventId);
    }

    public async Task<IEventCategory> AddAsync(IEventCategory newEvent)
    {
        var eventCategoryEntity = (EventCategory)newEvent;
        context.EventCategories.Add(eventCategoryEntity);
        await context.SaveChangesAsync();
        return (await this.GetByIdAsync(eventCategoryEntity.Id))!;
    }

    public async Task<IEventCategory?> UpdateAsync(IEventCategory updatedEvent)
    {
        var eventCategoryEntity = (EventCategory)updatedEvent;
        var existingEventCategory = await context.EventCategories.FindAsync(eventCategoryEntity.Id);
        if (existingEventCategory == null)
        {
            return null;
        }
        context.Entry(existingEventCategory).CurrentValues.SetValues(eventCategoryEntity);
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
}