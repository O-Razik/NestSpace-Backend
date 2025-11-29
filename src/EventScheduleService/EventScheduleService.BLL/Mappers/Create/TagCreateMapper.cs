using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Create;

namespace EventScheduleService.BLL.Mappers.Create;

public class TagCreateMapper(
    IEntityFactory<IEventTag> factory) : IShortMapper<IEventTag, TagCreateDto>
{
    public IEventTag ToEntity(TagCreateDto dto)
    {
        var tag = factory.CreateEntity();
        tag.SpaceId = dto.SpaceId;
        tag.Title = dto.Title;
        tag.Color = dto.Color;
        return tag;
    }
}