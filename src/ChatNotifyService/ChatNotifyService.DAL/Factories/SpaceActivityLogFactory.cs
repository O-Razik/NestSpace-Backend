using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.DAL.Factories;

public class SpaceActivityLogFactory : IEntityFactory<ISpaceActivityLog>
{
    public ISpaceActivityLog CreateEntity()
    {
        return new Entities.SpaceActivityLog();
    }
}