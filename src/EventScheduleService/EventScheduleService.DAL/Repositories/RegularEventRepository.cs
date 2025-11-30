using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class RegularEventRepository(EventScheduleDbContext context) : IRegularEventRepository
{
    public async Task<IEnumerable<IRegularEvent>> GetAllBySpaceAsync(Guid spaceId)
    {
        return await context.RegularEvents
            .Where(re => re.SpaceId == spaceId)
            .Include(re => re.Tags)
            .Include(re => re.Category)
            .ToListAsync();
    }

    public async Task<IRegularEvent?> GetByIdAsync(Guid id)
    {
        return await context.RegularEvents
            .Include(re => re.Tags)
            .Include(re => re.Category)
            .FirstOrDefaultAsync(re => re.Id == id);
    }

    public async Task<IRegularEvent> AddAsync(IRegularEvent entity)
    {
        var regularEvent = (RegularEvent)entity;
        foreach (var tag in regularEvent.Tags)
        {
            context.Entry(tag).State = EntityState.Unchanged;
        }
        
        context.RegularEvents.Add(regularEvent);
        await context.SaveChangesAsync();
        return (await GetByIdAsync(regularEvent.Id))!;
    }

    public async Task<IRegularEvent?> UpdateAsync(IRegularEvent entity)
    {
        var regularEvent = (RegularEvent)entity;

        var existingRegularEvent = await context.RegularEvents
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == regularEvent.Id && r.SpaceId == regularEvent.SpaceId);

        if (existingRegularEvent == null)
            return null;

        context.Entry(existingRegularEvent).CurrentValues.SetValues(new
        {
            regularEvent.Title,
            regularEvent.Description,
            regularEvent.SpaceId,
            regularEvent.Day,
            regularEvent.Frequency,
            regularEvent.CategoryId,
            regularEvent.StartTime,
            regularEvent.Duration
        });

        var newTagIds = regularEvent.Tags.Select(t => t.Id).ToList();
        var existingTagIds = existingRegularEvent.Tags.Select(t => t.Id).ToList();

        // Remove deleted tags
        var tagsToRemove = existingRegularEvent.Tags
            .Where(t => !newTagIds.Contains(t.Id))
            .ToList();

        foreach (var tag in tagsToRemove)
            existingRegularEvent.Tags.Remove(tag);

        // Add new tags
        var tagIdsToAdd = newTagIds
            .Where(newId => !existingTagIds.Contains(newId));

        foreach (var stub in tagIdsToAdd
                     .Select(tagId => new EventTag { Id = tagId }))
        {
            context.Entry(stub).State = EntityState.Unchanged;
            existingRegularEvent.Tags.Add(stub);
        }

        await context.SaveChangesAsync();
        return existingRegularEvent;
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var regularEvent = await context.RegularEvents.FindAsync(id);
        if (regularEvent == null)
        {
            return false;
        }
        context.RegularEvents.Remove(regularEvent);
        await context.SaveChangesAsync();
        return true;
    }
}