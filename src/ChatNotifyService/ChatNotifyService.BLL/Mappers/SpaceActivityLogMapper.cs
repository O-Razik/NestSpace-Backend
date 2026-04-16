using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Mappers;

public class SpaceActivityLogMapper :
    IMapper<SpaceActivityLog, SpaceActivityLogDto>,
    ICreateMapper<SpaceActivityLog, SpaceActivityLogCreateDto>
{
    public SpaceActivityLogDto ToDto(SpaceActivityLog source)
    {
        Guard.AgainstNull(source);
        return new SpaceActivityLogDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            MemberId = source.MemberId,
            Type = source.Type,
            Description = source.Description
        };
    }

    public SpaceActivityLog ToEntity(SpaceActivityLogDto dto)
    {
        Guard.AgainstNull(dto);
        return new SpaceActivityLog
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            MemberId = dto.MemberId,
            Type = dto.Type,
            Description = dto.Description
        };
    }

    public SpaceActivityLog ToEntity(SpaceActivityLogCreateDto dto)
    {
        Guard.AgainstNull(dto);
        return new SpaceActivityLog
        {
            SpaceId = dto.SpaceId,
            MemberId = dto.MemberId,
            Type = dto.Type,
            Description = dto.Description
        };
    }
}
