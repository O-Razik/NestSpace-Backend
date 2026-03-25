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

    public async Task<IRegularEvent?> GetByIdAsync(Guid regularEventId)
    {
        return await context.RegularEvents
            .Include(re => re.Tags)
            .Include(re => re.Category)
            .FirstOrDefaultAsync(re => re.Id == regularEventId);
    }

    public async Task<IRegularEvent> AddAsync(IRegularEvent newRegularEvent)
    {
        var regularEvent = new RegularEvent
        {
            Id = newRegularEvent.Id == Guid.Empty ? Guid.NewGuid() : newRegularEvent.Id,
            Title = newRegularEvent.Title,
            Description = newRegularEvent.Description,
            SpaceId = newRegularEvent.SpaceId,
            Day = newRegularEvent.Day,
            Frequency = newRegularEvent.Frequency,
            CategoryId = newRegularEvent.CategoryId,
            StartTime = newRegularEvent.StartTime,
            Duration = newRegularEvent.Duration
        };
        
        foreach (var tag in regularEvent.Tags)
        {
            context.Entry(tag).State = EntityState.Unchanged;
        }
        
        context.RegularEvents.Add(regularEvent);
        await context.SaveChangesAsync();
        return regularEvent;
    }

    public async Task<IRegularEvent?> UpdateAsync(IRegularEvent updatedRegularEvent)
    {
        var existingRegularEvent = await context.RegularEvents
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => 
                r.Id == updatedRegularEvent.Id && 
                r.SpaceId == updatedRegularEvent.SpaceId);

        if (existingRegularEvent == null)
        {
            return null;
        }

        context.Entry(existingRegularEvent).CurrentValues.SetValues(new
        {
            updatedRegularEvent.Title,
            updatedRegularEvent.Description,
            updatedRegularEvent.SpaceId,
            updatedRegularEvent.Day,
            updatedRegularEvent.Frequency,
            updatedRegularEvent.CategoryId,
            updatedRegularEvent.StartTime,
            updatedRegularEvent.Duration
        });

        var newTagIds = updatedRegularEvent.Tags
            .Select(t => t.Id).ToList();
        var existingTagIds = existingRegularEvent.Tags
            .Select(t => t.Id).ToList();

        // Remove deleted tags
        var tagsToRemove = existingRegularEvent.Tags
            .Where(t => !newTagIds.Contains(t.Id))
            .ToList();

        foreach (var tag in tagsToRemove)
        {
            existingRegularEvent.Tags.Remove(tag);
        }

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
    
    public async Task<bool> DeleteAsync(Guid regularEventId)
    {
        var regularEvent = await context.RegularEvents.FindAsync(regularEventId);
        if (regularEvent == null)
        {
            return false;
        }
        context.RegularEvents.Remove(regularEvent);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBySpaceIdAsync(Guid spaceId)
    {
        var regularEvents = await context.RegularEvents
            .Where(re => re.SpaceId == spaceId)
            .ToListAsync();
        if (regularEvents.Count == 0)
        {
            return false;
        }
        context.RegularEvents.RemoveRange(regularEvents);
        await context.SaveChangesAsync();
        return true;
    }
}
