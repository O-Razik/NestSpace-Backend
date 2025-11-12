using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;

namespace ChatNotifyService.BLL.Services;

public class MessageService(
    IMessageRepository messageRepository,
    IMessageReadRepository messageReadRepository,
    IChatNotificationService chatNotificationService
) : IMessageService
{
    public async Task<IEnumerable<IMessage>> GetAllMessagesAsync(Guid chatId, int pageNumber, int pageSize)
    {
        if (chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));

        if (pageNumber < 1)
            throw new ArgumentException("PageNumber must be greater than zero", nameof(pageNumber));

        return await messageRepository.GetAllAsync(chatId, pageNumber, pageSize);
    }

    public async Task<IEnumerable<IMessage>> GetRecentMessagesAsync(Guid chatId, int count = 20)
    {
        if (chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));

        if (count < 1)
            throw new ArgumentException("Count must be greater than zero", nameof(count));

        return await messageRepository.GetRecentMessagesAsync(chatId, count);
    }

    public async Task<IEnumerable<IMessage>> GetUnreadMessagesAsync(Guid chatId, Guid userId)
    {
        if (chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await messageRepository.GetUnreadMessagesAsync(chatId, userId);
    }

    public async Task<IMessage?> GetMessageByIdAsync(Guid messageId)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty", nameof(messageId));

        return await messageRepository.GetByIdAsync(messageId);
    }

    public async Task<IMessage> SendMessageAsync(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        message.Id = Guid.NewGuid();
        message.SentAt = DateTime.UtcNow;

        var createdMessage = await messageRepository.CreateAsync(message);
        await chatNotificationService.NotifyMessageSentAsync(createdMessage);

        return createdMessage;
    }

    public async Task<IMessage?> EditMessageAsync(IMessage updatedMessage)
    {
        ArgumentNullException.ThrowIfNull(updatedMessage);

        if (updatedMessage.Id == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty", nameof(updatedMessage));

        updatedMessage.ModifiedAt = DateTime.UtcNow;

        var message = await messageRepository.UpdateAsync(updatedMessage);

        if (message != null)
            await chatNotificationService.NotifyMessageEditedAsync(message);

        return message;
    }

    public async Task<IMessageRead> MarkMessageAsReadAsync(Guid messageId, Guid readerId)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty", nameof(messageId));

        if (readerId == Guid.Empty)
            throw new ArgumentException("ReaderId cannot be empty", nameof(readerId));

        var read = await messageReadRepository.MarkAsReadAsync(messageId, readerId);

        await chatNotificationService.NotifyMessageReadAsync(messageId, readerId);

        return read;
    }

    public async Task<IEnumerable<IMessageRead>> MarkMessagesAsReadAsync(Guid chatId, Guid readerId, DateTime upToTime)
    {
        if (chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));

        if (readerId == Guid.Empty)
            throw new ArgumentException("ReaderId cannot be empty", nameof(readerId));

        var unreadMessages = await messageRepository.GetUnreadMessagesAsync(chatId, readerId);
        var messagesToMark = unreadMessages.Where(m => m.SentAt <= upToTime);

        return await messageReadRepository.MarkAsReadsAsync(messagesToMark, readerId);
    }


    public async Task<bool> DeleteMessageAsync(Guid messageId)
    {
        if (messageId == Guid.Empty)
            throw new ArgumentException("MessageId cannot be empty", nameof(messageId));

        var deleted = await messageRepository.DeleteAsync(messageId);

        if (deleted)
            await chatNotificationService.NotifyMessageDeletedAsync(messageId);

        return deleted;
    }
}
