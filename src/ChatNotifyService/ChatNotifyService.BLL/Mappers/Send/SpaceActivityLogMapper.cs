using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Send;

namespace ChatNotifyService.BLL.Mappers.Send;

public class SpaceActivityLogMapper(
    IEntityFactory<ISpaceActivityLog> logFactory) :
    IMapper<ISpaceActivityLog, SpaceActivityLogDto>
{
    public SpaceActivityLogDto ToDto(ISpaceActivityLog source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new SpaceActivityLogDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            MemberId = source.MemberId,
            Type = source.Type,
            Description = source.Description
        };
    }

    public ISpaceActivityLog ToEntity(SpaceActivityLogDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var log = logFactory.CreateEntity();
        log.SpaceId = dto.SpaceId;
        log.MemberId = dto.MemberId;
        log.Type = dto.Type;
        log.Description = dto.Description;
        return log;
    }
}