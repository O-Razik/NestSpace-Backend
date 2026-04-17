using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Mappers;

public class MessageReadMapper(
    ChatMemberMapper chatMemberMapper,
    IDateTimeProvider dateTimeProvider)
    : IBigMapper<MessageRead, MessageReadDto, MessageReadDtoShort>
{
    public MessageReadDto ToDto(MessageRead source)
    {
        Guard.AgainstNull(source);
        return new MessageReadDto
        {
            MessageId = source.MessageId,
            ReaderId = source.ReaderId,
            ReadAt = source.ReadAt,
            Reader = chatMemberMapper.ToDto(source.Reader)
        };
    }

    public MessageRead ToEntity(MessageReadDto dto)
    {
        Guard.AgainstNull(dto);
        return new MessageRead
        {
            MessageId = dto.MessageId,
            ReaderId = dto.ReaderId,
            ReadAt = dto.ReadAt,
            Reader = chatMemberMapper.ToEntity(dto.Reader)
        };
    }
    
    public MessageReadDtoShort ToShortDto(MessageRead source)
    {
        Guard.AgainstNull(source);
        return new MessageReadDtoShort
        {
            MessageId = source.MessageId,
            ReaderId = source.ReaderId
        };
    }
    
    public MessageRead ToEntity(MessageReadDtoShort dto)
    {
        Guard.AgainstNull(dto);
        return new MessageRead
        {
            MessageId = dto.MessageId,
            ReaderId = dto.ReaderId,
            ReadAt = dateTimeProvider.UtcNow.DateTime,
        };
    }
}
