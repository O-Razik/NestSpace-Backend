using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Create;

namespace ChatNotifyService.BLL.Mappers.Create;

public class ChatMemberCreateMapper(
    IEntityFactory<IChatMember> memberFactory) :
    ICreateMapper<IChatMember, MemberCreateDto>
{
    public IChatMember ToEntity(Guid chatId, MemberCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var member = memberFactory.CreateEntity();
        member.ChatId = chatId;
        member.MemberId = dto.MemberId;
        member.PermissionLevel = dto.PermissionLevel;
        return member;
    }
}