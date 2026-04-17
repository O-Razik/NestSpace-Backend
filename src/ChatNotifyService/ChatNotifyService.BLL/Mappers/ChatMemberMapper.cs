using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Mappers;

public class ChatMemberMapper :
    IBigMapper<ChatMember, MemberDto, MemberDtoShort>
{
    public MemberDto ToDto(ChatMember source)
    {
        Guard.AgainstNull(source);
        return new MemberDto
        {
            ChatId = source.ChatId,
            MemberId = source.MemberId,
            PermissionLevel = source.PermissionLevel,
            JoinedAt = source.JoinedAt
        };
    }

    public ChatMember ToEntity(MemberDto dto)
    {
        Guard.AgainstNull(dto);
        return new ChatMember
        {
            ChatId = dto.ChatId,
            MemberId = dto.MemberId,
            PermissionLevel = dto.PermissionLevel,
            JoinedAt = dto.JoinedAt
        };
    }
    
    public MemberDtoShort ToShortDto(ChatMember source)
    {
        Guard.AgainstNull(source);
        return new MemberDtoShort
        {
            MemberId = source.MemberId,
            PermissionLevel = source.PermissionLevel,
            ChatId = source.ChatId
        };
    }
    
    public ChatMember ToEntity(MemberDtoShort dto)
    {
        Guard.AgainstNull(dto);
        return new ChatMember
        {
            ChatId = dto.ChatId,
            MemberId = dto.MemberId,
            PermissionLevel = dto.PermissionLevel
        };
    }
}
