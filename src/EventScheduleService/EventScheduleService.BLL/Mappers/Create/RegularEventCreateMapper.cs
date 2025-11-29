using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Create;

namespace EventScheduleService.BLL.Mappers.Create;

public class RegularEventCreateMapper(
    IEntityFactory<IRegularEvent> factory,
    IEntityFactory<IEventTag> tagFactory)
    : IShortMapper<IRegularEvent, RegularEventCreateDto>
{
    public IRegularEvent ToEntity(RegularEventCreateDto dto)
    {
        var regularEvent = factory.CreateEntity();
        regularEvent.SpaceId = dto.SpaceId;
        regularEvent.CategoryId = dto.CategoryId;
        regularEvent.Title = dto.Title;
        regularEvent.Description = dto.Description;
        regularEvent.StartTime = dto.StartTime;
        regularEvent.Duration = dto.Duration;
        regularEvent.Day = dto.Day;
        regularEvent.Frequency = dto.Frequency;
        regularEvent.Tags = dto.TagIds
            .Select(tagId =>
            {
                var tag = tagFactory.CreateEntity();
                tag.Id = tagId;
                return tag;
            }).ToList();
        return regularEvent;
    }
}