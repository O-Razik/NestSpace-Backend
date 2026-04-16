using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Services;

public class SpaceActivityLogService(
    ISpaceActivityLogRepository repository,
    ICreateMapper<SpaceActivityLog, SpaceActivityLogCreateDto> createMapper) : 
    ISpaceActivityLogService
{
    public async Task<IEnumerable<SpaceActivityLog>> GetActivityLogsBySpaceAsync(Guid spaceId, int limit, int offset)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstNegative(offset);
        Guard.AgainstNegativeOrZero(limit);
        return await repository.GetActivityLogsBySpaceAsync(spaceId, limit, offset);
    }

    public async Task<SpaceActivityLog> CreateActivityLogAsync(SpaceActivityLogCreateDto newActivityLog)
    {
        Guard.AgainstNull(newActivityLog);
        return await repository.CreateActivityLogAsync(createMapper.ToEntity(newActivityLog));
    }

    public async Task<bool> DeleteActivityLogsBySpaceIdAsync(Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        return await repository.DeleteActivityLogsBySpaceIdAsync(spaceId);
    }
}
