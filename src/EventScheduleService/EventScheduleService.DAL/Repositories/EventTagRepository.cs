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
        var tag = (EventTag)entity;
        context.EventTags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<IEventTag?> UpdateAsync(IEventTag entity)
    {
        var tag = (EventTag)entity;
        var existingMarker = await context.EventTags.FindAsync(tag.Id);
        if (existingMarker == null)
        {
            return null;
        }

        context.Entry(existingMarker).CurrentValues.SetValues(tag);
        await context.SaveChangesAsync();
        return existingMarker;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tag = await context.EventTags.FindAsync(id);
        if (tag == null)
        {
            return false;
        }

        context.EventTags.Remove(tag);
        await context.SaveChangesAsync();
        return true;
    }
}