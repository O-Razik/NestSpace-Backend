using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.BLL.Mappers.Send;

public class CategoryMapper(
    RegularEventMapper regularEventMapper,
    SoloEventMapper soloEventMapper)
    : IMapper<EventCategory, CategoryDto>
{
    public CategoryDto ToDto(EventCategory source)
    {
        return new CategoryDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            Title = source.Title,
            Description = source.Description,
            RegularEvents = source.RegularEvents
                .Select(regularEventMapper.ToDto).ToList(),
            SoloEvents = source.SoloEvents
                .Select(soloEventMapper.ToDto).ToList()
        };
    }

    public EventCategory ToEntity(CategoryDto dto)
    {
        return new EventCategory{
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            Title = dto.Title,
            Description = dto.Description,
            RegularEvents = dto.RegularEvents
                .Select(regularEventMapper.ToEntity).ToList(),
            SoloEvents = dto.SoloEvents
                .Select(soloEventMapper.ToEntity).ToList()
        };
    }
}

public class CategoryShortMapper(
    RegularEventMapper regularEventMapper,
    SoloEventMapper soloEventMapper)
    : IMapper<EventCategory, CategoryShortDto>
{

    public CategoryShortDto ToDto(EventCategory source)
    {
        return new CategoryShortDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            Title = source.Title
        };
    }

    public EventCategory ToEntity(CategoryShortDto dto)
    {
        return new EventCategory{
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            Title = dto.Title
        };
    }
}
