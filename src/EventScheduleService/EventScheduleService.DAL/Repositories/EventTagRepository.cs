using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.Models;
using EventScheduleService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class EventTagRepository(EventScheduleDbContext context) : IEventTagRepository
{
    public async Task<EventTag?> GetByIdAsync(Guid tagId)
    {
        return await context.EventTags
            .FirstOrDefaultAsync(m => m.Id == tagId);
    }

    public async Task<IEnumerable<EventTag>> GetAllAsync()
    {
        return await context.EventTags.ToListAsync();
    }

    public async Task<IEnumerable<EventTag>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.EventTags
            .Where(m => m.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task<EventTag> AddAsync(Guid spaceId, string title, string color)
    {
        var tag = new EventTag
        {
            Id = Guid.NewGuid(),
            SpaceId = spaceId,
            Title = title,
            Color = color
        };
        context.EventTags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<EventTag?> UpdateAsync(EventTag updatedTag)
    {
        var tag = new EventTag
        {
            Id = updatedTag.Id,
            SpaceId = updatedTag.SpaceId,
            Title = updatedTag.Title,
            Color = updatedTag.Color
        };
        
        var existingMarker = await context.EventTags.FindAsync(tag.Id);
        if (existingMarker == null)
        {
            return null;
        }

        context.Entry(existingMarker).CurrentValues.SetValues(tag);
        await context.SaveChangesAsync();
        return existingMarker;
    }

    public async Task<bool> DeleteAsync(Guid tagId)
    {
        var tag = await context.EventTags.FindAsync(tagId);
        if (tag == null)
        {
            return false;
        }

        context.EventTags.Remove(tag);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBySpaceIdAsync(Guid spaceId)
    {
        var tags = await context.EventTags
            .Where(m => m.SpaceId == spaceId)
            .ToListAsync();
        if (tags.Count == 0)
        {
            return false;
        }

        context.EventTags.RemoveRange(tags);
        await context.SaveChangesAsync();
        return true;
    }
}
