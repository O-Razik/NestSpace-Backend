using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Create;

namespace EventScheduleService.BLL.Mappers.Create;

public class SoloEventCreateMapper(
    IEntityFactory<ISoloEvent> factory,
    IEntityFactory<IEventTag> tagFactory
) : IShortMapper<ISoloEvent, SoloEventCreateDto>
{
    public ISoloEvent ToEntity(SoloEventCreateDto dto)
    {
        var soloEvent = factory.CreateEntity();
        soloEvent.SpaceId = dto.SpaceId;
        soloEvent.CategoryId = dto.CategoryId;
        soloEvent.Title = dto.Title;
        soloEvent.Description = dto.Description;
        soloEvent.StartDate = dto.StartDate;
        soloEvent.EndDate = dto.EndDate;
        soloEvent.IsYearly = dto.IsYearly;
        soloEvent.Tags = dto.TagIds
            .Select(tagId =>
            {
                var tag = tagFactory.CreateEntity();
                tag.Id = tagId;
                return tag;
            }).ToList();
        return soloEvent;
    }
}