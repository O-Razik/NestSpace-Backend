using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.DAL.Models;

namespace EventScheduleService.DAL.Factories;

public class CategoryFactory : IEntityFactory<IEventCategory>
{
    public IEventCategory CreateEntity()
    {
        return new EventCategory();
    }
}