using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Send;

namespace EventScheduleService.BLL.Mappers.Send;

public class SoloEventMapper(
    IEntityFactory<ISoloEvent> factory,
    TagMapper tagMapper
) : IMapper<ISoloEvent, SoloEventDto>
{
    public SoloEventDto ToDto(ISoloEvent source)
    {
        return new SoloEventDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            CategoryId = source.CategoryId,
            Title = source.Title,
            Description = source.Description,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            IsYearly = source.IsYearly,
            Tags = source.Tags.Select(tagMapper.ToDto).ToList()
        };
    }

    public ISoloEvent ToEntity(SoloEventDto dto)
    {
        var soloEvent = factory.CreateEntity();
        soloEvent.Id = dto.Id;
        soloEvent.SpaceId = dto.SpaceId;
        soloEvent.CategoryId = dto.CategoryId;
        soloEvent.Title = dto.Title;
        soloEvent.Description = dto.Description;
        soloEvent.StartDate = dto.StartDate;
        soloEvent.EndDate = dto.EndDate;
        soloEvent.IsYearly = dto.IsYearly;
        soloEvent.Tags = dto.Tags.Select(tagMapper.ToEntity).ToList();
        return soloEvent;
    }
}