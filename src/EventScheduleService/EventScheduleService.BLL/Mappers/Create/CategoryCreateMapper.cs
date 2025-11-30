using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Create;

namespace EventScheduleService.BLL.Mappers.Create;

public class CategoryCreateMapper(
    IEntityFactory<IEventCategory> factory)
    : ICreateMapper<IEventCategory, CategoryCreateDto>
{
    public IEventCategory ToEntity(Guid spaceId, CategoryCreateDto dto)
    {
        var category = factory.CreateEntity();
        category.SpaceId = spaceId;
        category.Title = dto.Tittle;
        category.Description = dto.Description;
        return category;
    }
}