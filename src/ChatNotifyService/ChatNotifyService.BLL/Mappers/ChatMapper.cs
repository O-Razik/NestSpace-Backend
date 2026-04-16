using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Mappers;

public class ChatMapper(
    ChatMemberMapper chatMemberMapper)
    : IBigMapper<Chat, ChatDto, ChatDtoShort>, ICreateMapper<Chat, ChatCreateDto>
{
    public ChatDto ToDto(Chat source)
    {
        Guard.AgainstNull(source);
        return new ChatDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            Name = source.Name,
            Members = source.Members
                .Select(chatMemberMapper.ToDto).ToList()
        };
    }

    public Chat ToEntity(ChatDto dto)
    {
        Guard.AgainstNull(dto);
        return new Chat
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            Name = dto.Name,
            Members = dto.Members
                .Select(chatMemberMapper.ToEntity).ToList()
        };
    }

    public ChatDtoShort ToShortDto(Chat source)
    {
        return new ChatDtoShort
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            Name = source.Name
        };
    }

    public Chat ToEntity(ChatDtoShort dto)
    {
        return new Chat
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            Name = dto.Name
        };
    }
    
    public Chat ToEntity(ChatCreateDto dto)
    {
        Guard.AgainstNull(dto);
        return new Chat
        {
            Id = Guid.NewGuid(),
            SpaceId = dto.SpaceId,
            Name = dto.Name,
            Members = dto.Members
                .Select(memberDto =>
                {
                    var member = new ChatMember
                    {
                        MemberId = memberDto.MemberId,
                        PermissionLevel = memberDto.PermissionLevel
                    };
                    return member;
                }).ToList()
        };
    }
}
