using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Send;

namespace ChatNotifyService.BLL.Mappers.Send;

public class MessageReadMapper(
    IEntityFactory<IMessageRead> messageReadFactory,
    ChatMemberMapper chatMemberMapper)
    : IBigMapper<IMessageRead, MessageReadDto, MessageReadDtoShort>
{
    public MessageReadDto ToDto(IMessageRead source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new MessageReadDto
        {
            MessageId = source.MessageId,
            ReaderId = source.ReaderId,
            ReadAt = source.ReadAt,
            Reader = chatMemberMapper.ToDto(source.Reader)
        };
    }

    public IMessageRead ToEntity(MessageReadDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var messageRead = messageReadFactory.CreateEntity();
        messageRead.MessageId = dto.MessageId;
        messageRead.ReaderId = dto.ReaderId;
        messageRead.ReadAt = dto.ReadAt;
        messageRead.Reader = chatMemberMapper.ToEntity(dto.Reader);
        return messageRead;
    }
    
    public MessageReadDtoShort ToShortDto(IMessageRead source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        return new MessageReadDtoShort
        {
            MessageId = source.MessageId,
            ReaderId = source.ReaderId
        };
    }
    
    public IMessageRead ToEntity(MessageReadDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var messageRead = messageReadFactory.CreateEntity();
        messageRead.MessageId = dto.MessageId;
        messageRead.ReaderId = dto.ReaderId;
        return messageRead;
    }
}