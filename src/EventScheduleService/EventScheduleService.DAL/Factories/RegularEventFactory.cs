using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.DAL.Models;

namespace EventScheduleService.DAL.Factories;

public class RegularEventFactory : IEntityFactory<IRegularEvent>
{
    public IRegularEvent CreateEntity()
    {
        return new RegularEvent();
    }
}