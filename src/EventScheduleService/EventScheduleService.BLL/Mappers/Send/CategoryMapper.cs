using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Send;

namespace EventScheduleService.BLL.Mappers.Send;

public class CategoryMapper(
    IEntityFactory<IEventCategory> factory,
    RegularEventMapper regularEventMapper,
    SoloEventMapper soloEventMapper)
    : IMapper<IEventCategory, CategoryDto, CategoryShortDto>
{
    public CategoryDto ToDto(IEventCategory source)
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

    public IEventCategory ToEntity(CategoryDto dto)
    {
        var category = factory.CreateEntity();
        category.Id = dto.Id;
        category.SpaceId = dto.SpaceId;
        category.Title = dto.Title;
        category.Description = dto.Description;
        category.RegularEvents = dto.RegularEvents
            .Select(regularEventMapper.ToEntity).ToList();
        category.SoloEvents = dto.SoloEvents
            .Select(soloEventMapper.ToEntity).ToList();
        return category;
    }

    public CategoryShortDto ToShortDto(IEventCategory source)
    {
        return new CategoryShortDto
        {
            Id = source.Id,
            Title = source.Title
        };
    }

    public IEventCategory ToEntity(CategoryShortDto dto)
    {
        var category = factory.CreateEntity();
        category.Id = dto.Id;
        category.SpaceId = dto.SpaceId;
        category.Title = dto.Title;
        return category;
    }
}