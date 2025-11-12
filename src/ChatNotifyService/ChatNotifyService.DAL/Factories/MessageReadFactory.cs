using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.DAL.Factories;

public class MessageReadFactory : IEntityFactory<IMessageRead>
{
    public IMessageRead CreateEntity()
    {
        return new Entities.MessageRead();
    }
}