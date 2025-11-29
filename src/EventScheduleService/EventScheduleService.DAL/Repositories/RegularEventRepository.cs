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
            .ToListAsync();
    }

    public async Task<IRegularEvent?> GetByIdAsync(Guid id)
    {
        return await context.RegularEvents
            .FirstOrDefaultAsync(re => re.Id == id);
    }

    public async Task<IRegularEvent> AddAsync(IRegularEvent entity)
    {
        var regularEvent = (RegularEvent)entity;
        context.RegularEvents.Add(regularEvent);
        await context.SaveChangesAsync();
        return regularEvent;
    }

    public async Task<IRegularEvent?> UpdateAsync(IRegularEvent entity)
    {
        var regularEvent = (RegularEvent)entity;
        var existingRegularEvent = await context.RegularEvents.FindAsync(regularEvent.Id);
        if (existingRegularEvent == null)
        {
            return null;
        }
        context.Entry(existingRegularEvent).CurrentValues.SetValues(regularEvent);
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