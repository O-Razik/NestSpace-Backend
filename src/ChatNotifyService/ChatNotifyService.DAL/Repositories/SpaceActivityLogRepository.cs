using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class SpaceActivityLogRepository(ChatNotifyDbContext context) : ISpaceActivityLogRepository
{
    public async Task<IEnumerable<ISpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int page, int amount)
    {
        return await context.SpaceActivityLogs.Where(log => log.SpaceId == spaceId)
            .OrderByDescending(log => log.Timestamp)
            .Skip((page - 1) * amount)
            .Take(amount)
            .ToListAsync();
    }

    public async Task<ISpaceActivityLog> CreateActivityLogAsync(ISpaceActivityLog newActivityLog)
    {
        var entity = (SpaceActivityLog)newActivityLog;
        await context.SpaceActivityLogs.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId)
    {
        var logs = context.SpaceActivityLogs.Where(log => log.SpaceId == spaceId);
        context.SpaceActivityLogs.RemoveRange(logs);
        var deleted = await context.SaveChangesAsync();
        return deleted > 0;
    }
}