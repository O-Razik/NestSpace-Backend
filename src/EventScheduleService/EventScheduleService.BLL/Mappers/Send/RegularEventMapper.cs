using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Send;

namespace EventScheduleService.BLL.Mappers.Send;

public class RegularEventMapper(
    IEntityFactory<IRegularEvent> factory,
    TagMapper tagMapper
) : IMapper<IRegularEvent, RegularEventDto>
{
    public RegularEventDto ToDto(IRegularEvent source)
    {
        return new RegularEventDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            CategoryId = source.CategoryId,
            Title = source.Title,
            Description = source.Description,
            StartTime = source.StartTime,
            Duration = source.Duration,
            Day = source.Day,
            Frequency = source.Frequency,
            Tags = source.Tags.Select(tagMapper.ToDto).ToList()
        };
    }

    public IRegularEvent ToEntity(RegularEventDto dto)
    {
        var regularEvent = factory.CreateEntity();
        regularEvent.Id = dto.Id;
        regularEvent.SpaceId = dto.SpaceId;
        regularEvent.CategoryId = dto.CategoryId;
        regularEvent.Title = dto.Title;
        regularEvent.Description = dto.Description;
        regularEvent.StartTime = dto.StartTime;
        regularEvent.Duration = dto.Duration;
        regularEvent.Day = dto.Day;
        regularEvent.Frequency = dto.Frequency;
        regularEvent.Tags = dto.Tags.Select(tagMapper.ToEntity).ToList();
        return regularEvent;
    }
}