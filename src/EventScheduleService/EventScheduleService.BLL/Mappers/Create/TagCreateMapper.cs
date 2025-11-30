using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Create;

namespace EventScheduleService.BLL.Mappers.Create;

public class TagCreateMapper(
    IEntityFactory<IEventTag> factory) : ICreateMapper<IEventTag, TagCreateDto>
{
    public IEventTag ToEntity(Guid spaceId, TagCreateDto dto)
    {
        var tag = factory.CreateEntity();
        tag.SpaceId = spaceId;
        tag.Title = dto.Title;
        tag.Color = dto.Color;
        return tag;
    }
}