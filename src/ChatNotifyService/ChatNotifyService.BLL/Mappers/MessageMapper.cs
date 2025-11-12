using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos;

namespace ChatNotifyService.BLL.Mappers;

public class MessageMapper (
    IEntityFactory<IMessage> messageFactory,
    ChatMemberMapper chatMemberMapper,
    MessageReadMapper messageReadMapper)
    : IBigMapper<IMessage, MessageDto, MessageDtoShort>
{
    public MessageDto ToDto(IMessage source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new MessageDto
        {
            Id = source.Id,
            ChatId = source.ChatId,
            SenderId = source.SenderId,
            Content = source.Content,
            SentAt = source.SentAt,
            ModifiedAt = source.ModifiedAt,
            IsEdited = source.IsEdited,
            IsDeleted = source.IsDeleted,
            ReplyToMessageId = source.ReplyToMessageId,
            Sender = chatMemberMapper.ToShortDto(source.Sender),
            Reads = source.Reads.Select(messageReadMapper.ToDto).ToList()
        };
    }

    public IMessage ToEntity(MessageDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var message = messageFactory.CreateEntity();
        message.Id = dto.Id;
        message.ChatId = dto.ChatId;
        message.SenderId = dto.SenderId;
        message.Content = dto.Content;
        message.SentAt = dto.SentAt;
        message.ModifiedAt = dto.ModifiedAt;
        message.IsEdited = dto.IsEdited;
        message.IsDeleted = dto.IsDeleted;
        message.ReplyToMessageId = dto.ReplyToMessageId;
        message.Sender = chatMemberMapper.ToEntity(dto.Sender);
        message.Reads = dto.Reads
            .Select(messageReadMapper.ToEntity).ToList();
        return message;
    }
    
    public MessageDtoShort ToShortDto(IMessage source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new MessageDtoShort
        {
            Id = source.Id,
            ChatId = source.ChatId,
            SenderId = source.SenderId,
            Content = source.Content,
            SentAt = source.SentAt,
            ModifiedAt = source.ModifiedAt,
            IsEdited = source.IsEdited,
            IsDeleted = source.IsDeleted,
            ReplyToMessageId = source.ReplyToMessageId
        };
    }
    
    public IMessage ToEntity(MessageDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var message = messageFactory.CreateEntity();
        message.Id = dto.Id;
        message.ChatId = dto.ChatId;
        message.SenderId = dto.SenderId;
        message.Content = dto.Content;
        message.SentAt = dto.SentAt;
        message.ModifiedAt = dto.ModifiedAt;
        message.IsEdited = dto.IsEdited;
        message.IsDeleted = dto.IsDeleted;
        message.ReplyToMessageId = dto.ReplyToMessageId;
        return message;
    }
}