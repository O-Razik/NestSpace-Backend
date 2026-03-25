using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.BLL.Mappers;

public class TagMapper : IMapper<EventTag, TagDto>
{
    public TagDto ToDto(EventTag source)
    {
        return new TagDto
        {
            Id = source.Id != Guid.Empty
                ? source.Id : Guid.NewGuid(),
            SpaceId = source.SpaceId,
            Title = source.Title,
            Color = source.Color,
        };
    }

    public EventTag ToEntity(TagDto dto)
    {
        return new EventTag
        {
            Id = dto.Id != Guid.Empty
                ? dto.Id : Guid.NewGuid(),
            SpaceId = dto.SpaceId,
            Title = dto.Title,
            Color = dto.Color,
        };
    }
}
