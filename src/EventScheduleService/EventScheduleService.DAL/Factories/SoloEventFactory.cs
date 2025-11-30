using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.DAL.Models;

namespace EventScheduleService.DAL.Factories;

public class SoloEventFactory : IEntityFactory<ISoloEvent>
{
    public ISoloEvent CreateEntity()
    {
        return new SoloEvent();
    }
}