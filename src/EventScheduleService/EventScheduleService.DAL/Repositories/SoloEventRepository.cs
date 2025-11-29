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
            .ToListAsync();
    }

    public async Task<ISoloEvent?> GetByIdAsync(Guid id)
    {
        return await context.SoloEvents
            .FirstOrDefaultAsync(se => se.Id == id);
    }

    public async Task<ISoloEvent> AddAsync(ISoloEvent entity)
    {
        var soloEvent = (SoloEvent)entity;
        context.SoloEvents.Add(soloEvent);
        await context.SaveChangesAsync();
        return soloEvent;
    }

    public async Task<ISoloEvent?> UpdateAsync(ISoloEvent entity)
    {
        var soloEvent = (SoloEvent)entity;
        var existingSoloEvent = await context.SoloEvents.FindAsync(soloEvent.Id);
        if (existingSoloEvent == null)
        {
            return null;
        }
        context.Entry(existingSoloEvent).CurrentValues.SetValues(soloEvent);
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