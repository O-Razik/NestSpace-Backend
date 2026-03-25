using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.BLL.Mappers;

public class SoloEventMapper(
    TagMapper tagMapper
) : IMapper<SoloEvent, SoloEventDto>, IEntityMapper<SoloEvent, CreateSoloEventDto>
{
    public SoloEventDto ToDto(SoloEvent source)
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

    public SoloEvent ToEntity(SoloEventDto dto)
    {
        return new SoloEvent
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsYearly = dto.IsYearly,
            Tags = dto.Tags.Select(tagMapper.ToEntity).ToList()
        };
    }

    public SoloEvent ToEntity(CreateSoloEventDto dto)
    {
        return new SoloEvent
        {
            Id = Guid.NewGuid(),
            SpaceId = dto.SpaceId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsYearly = dto.IsYearly,
            Tags = dto.Tags.Select(tagMapper.ToEntity).ToList()
        };
    }
}
