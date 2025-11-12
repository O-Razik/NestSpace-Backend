using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.DAL.Factories;

public class ChatMemberFactory : IEntityFactory<IChatMember>
{
    public IChatMember CreateEntity()
    {
        return new Entities.ChatMember();
    }
}