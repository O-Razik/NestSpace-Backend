using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;

namespace ChatNotifyService.BLL.Services;

public class SpaceActivityLogService(
    ISpaceActivityLogRepository repository) : ISpaceActivityLogService
{
    public Task<IEnumerable<ABS.IEntities.ISpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int limit, int offset)
    {
        return repository.GetActivityLogsBySpaceAsync(spaceId, limit, offset);
    }

    public Task<ABS.IEntities.ISpaceActivityLog> CreateActivityLogAsync(ABS.IEntities.ISpaceActivityLog newActivityLog)
    {
        return repository.CreateActivityLogAsync(newActivityLog);
    }

    public Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId)
    {
        return repository.DeleteActivityLogsBySpaceIdAsync(spaceId);
    }
}