using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class SpaceActivityLogRepository(ChatNotifyDbContext context) : ISpaceActivityLogRepository
{
    public async Task<IEnumerable<SpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int page, int amount)
    {
        return await context.SpaceActivityLogs.Where(log => log.SpaceId == spaceId)
            .OrderByDescending(log => log.Timestamp)
            .Skip((page - 1) * amount)
            .Take(amount)
            .ToListAsync();
    }

    public async Task<SpaceActivityLog> CreateActivityLogAsync(SpaceActivityLog newActivityLog)
    {
        await context.SpaceActivityLogs.AddAsync(newActivityLog);
        await context.SaveChangesAsync();
        return newActivityLog;
    }

    public async Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId)
    {
        var logs = context.SpaceActivityLogs.Where(log => log.SpaceId == spaceId);
        context.SpaceActivityLogs.RemoveRange(logs);
        var deleted = await context.SaveChangesAsync();
        return deleted > 0;
    }
}
