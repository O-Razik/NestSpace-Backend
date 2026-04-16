using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IServices;

public interface ISpaceActivityLogService
{
    Task<IEnumerable<SpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int limit, int offset);
    
    Task<SpaceActivityLog> CreateActivityLogAsync(SpaceActivityLogCreateDto newActivityLog);
    
    Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId);
}
