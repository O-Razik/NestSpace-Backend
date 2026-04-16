

using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IRepositories;

public interface ISpaceActivityLogRepository
{
    Task<IEnumerable<SpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int page, int amount);
    
    Task<SpaceActivityLog> CreateActivityLogAsync(SpaceActivityLog newActivityLog);
    
    Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId);
}
