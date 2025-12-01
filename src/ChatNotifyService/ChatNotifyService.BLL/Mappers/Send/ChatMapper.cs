using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Send;

namespace ChatNotifyService.BLL.Mappers.Send;

public class ChatMapper(
    IEntityFactory<IChat> chatFactory,
    ChatMemberMapper chatMemberMapper)
    : IBigMapper<IChat, ChatDto, ChatDtoShort>
{
    public ChatDto ToDto(IChat source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new ChatDto
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            Name = source.Name,
            Members = source.Members
                .Select(chatMemberMapper.ToDto).ToList()
        };
    }

    public IChat ToEntity(ChatDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var chat = chatFactory.CreateEntity();
        chat.Id = dto.Id;
        chat.SpaceId = dto.SpaceId;
        chat.Name = dto.Name;
        chat.Members = dto.Members
            .Select(chatMemberMapper.ToEntity).ToList();
        return chat;
    }

    public ChatDtoShort ToShortDto(IChat source)
    {
        return new ChatDtoShort
        {
            Id = source.Id,
            SpaceId = source.SpaceId,
            Name = source.Name
        };
    }

    public IChat ToEntity(ChatDtoShort dto)
    {
        var chat = chatFactory.CreateEntity();
        chat.Id = dto.Id;
        chat.SpaceId = dto.SpaceId;
        chat.Name = dto.Name;
        return chat;
    }
}