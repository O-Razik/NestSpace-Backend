using EventScheduleService.ABS.Dtos;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.BLL.Mappers;

public class RegularEventMapper(
    TagMapper tagMapper
) : IMapper<RegularEvent, RegularEventDto> , IEntityMapper<RegularEvent, RegularEventCreateDto>
    
{
    public RegularEventDto ToDto(RegularEvent source)
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

    public RegularEvent ToEntity(RegularEventDto dto)
    {
        return new RegularEvent
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            Duration = dto.Duration,
            Day = dto.Day,
            Frequency = dto.Frequency,
            Tags = dto.Tags.Select(tagMapper.ToEntity).ToList()
        };
    }

    public RegularEvent ToEntity(RegularEventCreateDto createDto)
    {
        return new RegularEvent
        {
            Id = Guid.NewGuid(),
            SpaceId = createDto.SpaceId,
            CategoryId = createDto.CategoryId,
            Title = createDto.Title,
            Description = createDto.Description,
            StartTime = createDto.StartTime,
            Duration = createDto.Duration,
            Day = createDto.Day,
            Frequency = createDto.Frequency,
            Tags = createDto.Tags.Select(tagMapper.ToEntity).ToList()
        };
    }
}
