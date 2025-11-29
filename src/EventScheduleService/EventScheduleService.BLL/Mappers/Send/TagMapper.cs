using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Send;

namespace EventScheduleService.BLL.Mappers.Send;

public class TagMapper(IEntityFactory<IEventTag> factory) : IMapper<IEventTag, TagDto>
{
    public TagDto ToDto(IEventTag source)
    {
        return new TagDto
        {
            Id = source.Id,
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