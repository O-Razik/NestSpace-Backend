using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Create;

namespace ChatNotifyService.BLL.Mappers.Create;

public class SpaceActivityLogCreateMapper(
    IEntityFactory<ISpaceActivityLog> logFactory) :
    ICreateMapper<ISpaceActivityLog, SpaceActivityLogCreateDto>
{
    public ISpaceActivityLog ToEntity(Guid spaceId, SpaceActivityLogCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var log = logFactory.CreateEntity();
        log.SpaceId = spaceId;
        log.MemberId = dto.MemberId;
        log.Type = dto.Type;
        log.Description = dto.Description;
        return log;
    }
}