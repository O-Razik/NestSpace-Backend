using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.BLL.Mappers;

public class SoloEventMapper(
    IEntityFactory<ISoloEvent> factory,
    TagMapper tagMapper
) : IMapper<ISoloEvent, SoloEventDto>, IEntityMapper<ISoloEvent, CreateSoloEventDto>
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
        soloEvent.Tags = dto.Tags
            .Select(tagMapper.ToEntity).ToList();
        return soloEvent;
    }

    public ISoloEvent ToEntity(CreateSoloEventDto dto)
    {
        var soloEvent = factory.CreateEntity();
        soloEvent.SpaceId = Guid.NewGuid();
        soloEvent.CategoryId = dto.CategoryId;
        soloEvent.Title = dto.Title;
        soloEvent.Description = dto.Description;
        soloEvent.StartDate = dto.StartDate;
        soloEvent.EndDate = dto.EndDate;
        soloEvent.IsYearly = dto.IsYearly;
        soloEvent.Tags = dto.Tags
            .Select(tagMapper.ToEntity).ToList();
        return soloEvent;
    }
}
