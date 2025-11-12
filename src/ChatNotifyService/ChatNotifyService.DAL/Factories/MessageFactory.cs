using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.DAL.Factories;

public class MessageFactory : IEntityFactory<IMessage>
{
    public IMessage CreateEntity()
    {
        return new Entities.Message();
    }
}