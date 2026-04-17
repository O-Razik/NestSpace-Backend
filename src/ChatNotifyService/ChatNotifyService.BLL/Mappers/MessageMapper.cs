using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Mappers;

public class MessageMapper (
    ChatMemberMapper chatMemberMapper,
    MessageReadMapper messageReadMapper,
    IDateTimeProvider dateTimeProvider)
    : IBigMapper<Message, MessageDto, MessageDtoShort>, ICreateMapper<Message, MessageCreateDto>
{
    public MessageDto ToDto(Message source)
    {
        Guard.AgainstNull(source);
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

    public Message ToEntity(MessageDto dto)
    {
        Guard.AgainstNull(dto);
        return new Message
        {
            Id = dto.Id,
            ChatId = dto.ChatId,
            SenderId = dto.SenderId,
            Content = dto.Content,
            SentAt = dto.SentAt,
            ModifiedAt = dto.ModifiedAt,
            IsEdited = dto.IsEdited,
            IsDeleted = dto.IsDeleted,
            ReplyToMessageId = dto.ReplyToMessageId,
            Sender = chatMemberMapper.ToEntity(dto.Sender),
            Reads = dto.Reads.Select(messageReadMapper.ToEntity).ToList()
        };
    }
    
    public MessageDtoShort ToShortDto(Message source)
    {
        Guard.AgainstNull(source);
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
    
    public Message ToEntity(MessageDtoShort dto)
    {
        Guard.AgainstNull(dto);
        return new Message
        {
            Id = dto.Id,
            ChatId = dto.ChatId,
            SenderId = dto.SenderId,
            Content = dto.Content,
            SentAt = dto.SentAt,
            ModifiedAt = dto.ModifiedAt,
            IsEdited = dto.IsEdited,
            IsDeleted = dto.IsDeleted,
            ReplyToMessageId = dto.ReplyToMessageId
        };
    }
    
    public Message ToEntity(MessageCreateDto dto)
    {
        Guard.AgainstNull(dto);
        return new Message
        {
            Id = Guid.NewGuid(),
            ChatId = dto.ChatId,
            SenderId = dto.SenderId,
            Content = dto.Content,
            SentAt = dateTimeProvider.UtcNow.DateTime,
            ModifiedAt = dateTimeProvider.UtcNow.DateTime,
            IsEdited = false,
            IsDeleted = false,
            ReplyToMessageId = dto.ReplyToMessageId ?? null
        };
    }
}
