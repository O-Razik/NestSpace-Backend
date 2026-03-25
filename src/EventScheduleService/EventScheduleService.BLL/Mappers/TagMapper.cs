using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.BLL.Mappers;

public class TagMapper(IEntityFactory<IEventTag> factory) : IMapper<IEventTag, TagDto>
{
    public TagDto ToDto(IEventTag source)
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

    public IEventTag ToEntity(TagDto dto)
    {
        var tag = factory.CreateEntity();
        tag.Id = dto.Id;
        tag.SpaceId = dto.SpaceId;
        tag.Title = dto.Title;
        tag.Color = dto.Color;
        return tag;
    }
}
