using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.DAL.Models;

namespace EventScheduleService.DAL.Factories;

public class TagFactory : IEntityFactory<IEventTag>
{
    public IEventTag CreateEntity()
    {
        return new EventTag();
    }
}
