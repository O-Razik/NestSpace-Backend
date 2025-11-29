using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class EventTagRepository(EventScheduleDbContext context) : IEventTagRepository
{
    public async Task<IEventTag?> GetByIdAsync(Guid id)
    {
        return await context.EventTags
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<IEventTag>> GetAllAsync()
    {
        return await context.EventTags.ToListAsync();
    }

    public async Task<IEnumerable<IEventTag>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.EventTags
            .Where(m => m.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task<IEventTag> AddAsync(IEventTag entity)
    {
        var marker = (EventTag)entity;
        context.EventTags.Add(marker);
        await context.SaveChangesAsync();
        return marker;
    }

    public async Task<IEventTag?> UpdateAsync(IEventTag entity)
    {
        var marker = (EventTag)entity;
        var existingMarker = await context.EventTags.FindAsync(marker.Id);
        if (existingMarker == null)
        {
            return null;
        }

        context.Entry(existingMarker).CurrentValues.SetValues(marker);
        await context.SaveChangesAsync();
        return existingMarker;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var marker = await context.EventTags.FindAsync(id);
        if (marker == null)
        {
            return false;
        }

        context.EventTags.Remove(marker);
        await context.SaveChangesAsync();
        return true;
    }
}