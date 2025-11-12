using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos;

namespace ChatNotifyService.BLL.Mappers;

public class ChatMemberMapper(IEntityFactory<IChatMember> chatMemberFactory)
    : IBigMapper<IChatMember, ChatMemberDto, ChatMemberDtoShort>
{
    public ChatMemberDto ToDto(IChatMember source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new ChatMemberDto
        {
            ChatId = source.ChatId,
            MemberId = source.MemberId,
            JoinedAt = source.JoinedAt
        };
    }

    public IChatMember ToEntity(ChatMemberDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var chatMember = chatMemberFactory.CreateEntity();
        chatMember.ChatId = dto.ChatId;
        chatMember.MemberId = dto.MemberId;
        chatMember.JoinedAt = dto.JoinedAt;
        return chatMember;
    }
    
    public ChatMemberDtoShort ToShortDto(IChatMember source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new ChatMemberDtoShort
        {
            MemberId = source.MemberId,
            ChatId = source.ChatId
        };
    }
    
    public IChatMember ToEntity(ChatMemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var chatMember = chatMemberFactory.CreateEntity();
        chatMember.ChatId = dto.ChatId;
        chatMember.MemberId = dto.MemberId;
        return chatMember;
    }
}