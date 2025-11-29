using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Create;

namespace EventScheduleService.BLL.Mappers.Create;

public class CategoryCreateMapper(
    IEntityFactory<IEventCategory> factory)
    : IShortMapper<IEventCategory, CategoryCreateDto>
{
    public IEventCategory ToEntity(CategoryCreateDto dto)
    {
        var category = factory.CreateEntity();
        category.SpaceId = dto.SpaceId;
        category.Title = dto.Tittle;
        category.Description = dto.Description;
        return category;
    }
}