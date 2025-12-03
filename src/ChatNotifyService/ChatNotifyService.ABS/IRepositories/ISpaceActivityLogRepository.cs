using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IRepositories;

public interface ISpaceActivityLogRepository
{
    Task<IEnumerable<ISpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int limit, int offset);
    
    Task<ISpaceActivityLog> CreateActivityLogAsync(ISpaceActivityLog newActivityLog);
    
    Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId);
}