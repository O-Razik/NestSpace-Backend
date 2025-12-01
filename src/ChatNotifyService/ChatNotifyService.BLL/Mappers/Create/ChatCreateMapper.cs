using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Create;

namespace ChatNotifyService.BLL.Mappers.Create;

public class ChatCreateMapper(
    IEntityFactory<IChat> chatFactory,
    IEntityFactory<IChatMember> chatMemberFactory
    ) : ICreateMapper<IChat, ChatCreateDto>
{
    public IChat ToEntity(Guid spaceId, ChatCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var chat = chatFactory.CreateEntity();
        chat.SpaceId = spaceId;
        chat.Name = dto.Name;
        chat.Members = dto.Members
            .Select(memberDto =>
            {
                var member = chatMemberFactory.CreateEntity();
                member.MemberId = memberDto.MemberId;
                member.PermissionLevel = memberDto.PermissionLevel;
                return member;
            }).ToList();
        return chat;
    }
}