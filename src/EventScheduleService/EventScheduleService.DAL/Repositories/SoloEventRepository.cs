using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.DAL.Data;
using EventScheduleService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class SoloEventRepository(EventScheduleDbContext context) : ISoloEventRepository
{
    public async Task<IEnumerable<ISoloEvent>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.SoloEvents
            .Where(se => se.SpaceId == spaceId)
            .Include(se => se.Tags)
            .Include(se => se.Category)
            .ToListAsync();
    }

    public async Task<ISoloEvent?> GetByIdAsync(Guid id)
    {
        return await context.SoloEvents
            .Include(se => se.Tags)
            .Include(se => se.Category)
            .FirstOrDefaultAsync(se => se.Id == id);
    }

    public async Task<ISoloEvent> AddAsync(ISoloEvent entity)
    {
        var soloEvent = (SoloEvent)entity;
        foreach (var tag in entity.Tags)
        {
            context.Entry(tag).State = EntityState.Unchanged;
        }
        
        context.SoloEvents.Add(soloEvent);
        await context.SaveChangesAsync();
        return (await GetByIdAsync(soloEvent.Id))!;
    }

    public async Task<ISoloEvent?> UpdateAsync(ISoloEvent entity)
    {
        var soloEvent = (SoloEvent)entity;
        var existingSoloEvent = await context.SoloEvents
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == soloEvent.Id && e.SpaceId == soloEvent.SpaceId);

        if (existingSoloEvent == null)
            return null;

        context.Entry(existingSoloEvent).CurrentValues.SetValues(new
        {
            soloEvent.Title,
            soloEvent.Description,
            soloEvent.SpaceId,
            soloEvent.CategoryId,
            soloEvent.StartDate,
            soloEvent.EndDate,
            soloEvent.IsYearly
        });

        var newTagIds = soloEvent.Tags.Select(t => t.Id).ToList();
        var existingTagIds = existingSoloEvent.Tags.Select(t => t.Id).ToList();

        // Remove deleted tags
        var tagsToRemove = existingSoloEvent.Tags
            .Where(t => !newTagIds.Contains(t.Id))
            .ToList();

        foreach (var tag in tagsToRemove)
            existingSoloEvent.Tags.Remove(tag);

        // Add new tags
        var tagIdsToAdd = newTagIds
            .Where(newId => !existingTagIds.Contains(newId))
            .ToList();

        foreach (var stub in tagIdsToAdd
                     .Select(tagId => new EventTag { Id = tagId }))
        {
            context.Entry(stub).State = EntityState.Unchanged;
            existingSoloEvent.Tags.Add(stub);
        }

        await context.SaveChangesAsync();
        return existingSoloEvent;
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        var soloEvent = await context.SoloEvents.FindAsync(id);
        if (soloEvent == null)
        {
            return false;
        }
        context.SoloEvents.Remove(soloEvent);
        await context.SaveChangesAsync();
        return true;
    }
}