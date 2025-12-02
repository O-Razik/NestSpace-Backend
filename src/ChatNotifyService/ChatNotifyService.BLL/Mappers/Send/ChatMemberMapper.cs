using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Send;

namespace ChatNotifyService.BLL.Mappers.Send;

public class ChatMemberMapper(IEntityFactory<IChatMember> chatMemberFactory)
    : IBigMapper<IChatMember, MemberDto, MemberDtoShort>
{
    public MemberDto ToDto(IChatMember source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new MemberDto
        {
            ChatId = source.ChatId,
            MemberId = source.MemberId,
            PermissionLevel = source.PermissionLevel,
            JoinedAt = source.JoinedAt
        };
    }

    public IChatMember ToEntity(MemberDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var chatMember = chatMemberFactory.CreateEntity();
        chatMember.ChatId = dto.ChatId;
        chatMember.MemberId = dto.MemberId;
        chatMember.PermissionLevel = dto.PermissionLevel;
        chatMember.JoinedAt = dto.JoinedAt;
        return chatMember;
    }
    
    public MemberDtoShort ToShortDto(IChatMember source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new MemberDtoShort
        {
            MemberId = source.MemberId,
            PermissionLevel = source.PermissionLevel,
            ChatId = source.ChatId
        };
    }
    
    public IChatMember ToEntity(MemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var chatMember = chatMemberFactory.CreateEntity();
        chatMember.ChatId = dto.ChatId;
        chatMember.MemberId = dto.MemberId;
        chatMember.PermissionLevel = dto.PermissionLevel;
        return chatMember;
    }
}