using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.Models;
using EventScheduleService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace EventScheduleService.DAL.Repositories;

public class SoloEventRepository(EventScheduleDbContext context) : ISoloEventRepository
{
    public async Task<IEnumerable<SoloEvent>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.SoloEvents
            .Where(se => se.SpaceId == spaceId)
            .Include(se => se.Tags)
            .Include(se => se.Category)
            .ToListAsync();
    }

    public async Task<SoloEvent?> GetByIdAsync(Guid soloEventId)
    {
        return await context.SoloEvents
            .Include(se => se.Tags)
            .Include(se => se.Category)
            .FirstOrDefaultAsync(se => se.Id == soloEventId);
    }

    public async Task<SoloEvent> AddAsync(SoloEvent newSoloEvent)
    {
        var soloEventEntity = new SoloEvent
        {
            Id = newSoloEvent.Id == Guid.Empty ? 
                Guid.NewGuid() : 
                newSoloEvent.Id,
            Title = newSoloEvent.Title,
            Description = newSoloEvent.Description,
            SpaceId = newSoloEvent.SpaceId,
            CategoryId = newSoloEvent.CategoryId,
            StartDate = newSoloEvent.StartDate,
            EndDate = newSoloEvent.EndDate,
            IsYearly = newSoloEvent.IsYearly
        };
        
        foreach (var tag in soloEventEntity.Tags)
        {
            context.Entry(tag).State = EntityState.Unchanged;
        }

        context.SoloEvents.Add(soloEventEntity);
        await context.SaveChangesAsync();
        return soloEventEntity;
    }


    public async Task<SoloEvent?> UpdateAsync(SoloEvent updatedSoloEvent)
    {
        var existingSoloEvent = await context.SoloEvents
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == updatedSoloEvent.Id && e.SpaceId == updatedSoloEvent.SpaceId);

        if (existingSoloEvent == null)
        {
            return null;
        }

        context.Entry(existingSoloEvent).CurrentValues.SetValues(new
        {
            updatedSoloEvent.Title,
            updatedSoloEvent.Description,
            updatedSoloEvent.SpaceId,
            updatedSoloEvent.CategoryId,
            updatedSoloEvent.StartDate,
            updatedSoloEvent.EndDate,
            updatedSoloEvent.IsYearly
        });

        var newTagIds = updatedSoloEvent.Tags.Select(t => t.Id).ToList();
        var existingTagIds = existingSoloEvent.Tags.Select(t => t.Id).ToList();

        // Remove deleted tags
        var tagsToRemove = existingSoloEvent.Tags
            .Where(t => !newTagIds.Contains(t.Id))
            .ToList();

        foreach (var tag in tagsToRemove)
        {
            existingSoloEvent.Tags.Remove(tag);
        }

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


    public async Task<bool> DeleteAsync(Guid soloEventId)
    {
        var soloEvent = await context.SoloEvents.FindAsync(soloEventId);
        if (soloEvent == null)
        {
            return false;
        }
        context.SoloEvents.Remove(soloEvent);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBySpaceIdAsync(Guid spaceId)
    {
        var soloEvents = await context.SoloEvents
            .Where(se => se.SpaceId == spaceId)
            .ToListAsync();
        if (soloEvents.Count == 0)
        {
            return false;
        }
        context.SoloEvents.RemoveRange(soloEvents);
        await context.SaveChangesAsync();
        return true;
    }
}
