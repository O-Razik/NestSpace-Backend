using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.BLL.Dtos.Create;

namespace ChatNotifyService.BLL.Mappers.Create;

public class MessageCreateMapper(
    IEntityFactory<IMessage> messageFactory) :
    ICreateMapper<IMessage, MessageCreateDto>
{
    public IMessage ToEntity(Guid chatId, MessageCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var message = messageFactory.CreateEntity();
        message.ChatId = chatId;
        message.ReplyToMessageId = dto.ReplyToMessageId ?? null;
        message.SenderId = dto.SenderId;
        message.Content = dto.Content;
        return message;
    }
}