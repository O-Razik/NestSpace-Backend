using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IServices;

public interface ISpaceActivityLogService
{
    Task<IEnumerable<ISpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int limit, int offset);
    
    Task<ISpaceActivityLog> CreateActivityLogAsync(ISpaceActivityLog newActivityLog);
    
    Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId);
}