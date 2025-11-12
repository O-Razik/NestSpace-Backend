using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.DAL.Factories;

public class ChatFactory : IEntityFactory<IChat>
{
    public IChat CreateEntity()
    {
        return new Entities.Chat();
    }
}