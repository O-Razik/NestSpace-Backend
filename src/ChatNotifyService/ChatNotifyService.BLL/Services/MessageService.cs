using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Services;

public class MessageService(
    IMessageRepository messageRepository,
    IMessageReadRepository messageReadRepository,
    ICreateMapper<Message, MessageCreateDto> messageCreateMapper,
    IChatNotificationService chatNotificationService,
    IDateTimeProvider dateTimeProvider
) : IMessageService
{
    private const int MinPageSize = 10;
    private const int MaxPageSize = 100;
    
    public async Task<IEnumerable<Message>> GetAllMessagesAsync(Guid chatId, int pageNumber, int pageSize)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstOutOfRange(pageNumber, 1, int.MaxValue);
        Guard.AgainstOutOfRange(pageSize, MinPageSize, MaxPageSize);
        return await messageRepository.GetAllAsync(chatId, pageNumber, pageSize);
    }

    public async Task<IEnumerable<Message>> GetRecentMessagesAsync(Guid chatId, int count = 20)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstOutOfRange(count, 1, int.MaxValue);
        return await messageRepository.GetRecentMessagesAsync(chatId, count);
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid chatId, Guid userId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(userId);
        return await messageRepository.GetUnreadMessagesAsync(chatId, userId);
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId)
    {
        Guard.AgainstEmptyGuid(messageId);
        return await messageRepository.GetByIdAsync(messageId);
    }

    public async Task<Message> SendMessageAsync(MessageCreateDto message)
    {
        Guard.AgainstNull(message);
        var createdMessage = await messageRepository.CreateAsync(messageCreateMapper.ToEntity(message));
        await chatNotificationService.NotifyMessageSentAsync(createdMessage);
        return createdMessage;
    }

    public async Task<Message?> EditMessageAsync(Message updatedMessage)
    {
        Guard.AgainstNull(updatedMessage);
        Guard.AgainstEmptyGuid(updatedMessage.Id);

        updatedMessage.ModifiedAt = dateTimeProvider.UtcNow.DateTime;

        var message = await messageRepository.UpdateAsync(updatedMessage);

        if (message != null)
        {
            await chatNotificationService.NotifyMessageEditedAsync(message);
        }

        return message;
    }

    public async Task<MessageRead> MarkMessageAsReadAsync(Guid messageId, Guid readerId)
    {
        Guard.AgainstEmptyGuid(messageId);
        Guard.AgainstEmptyGuid(readerId);

        var read = await messageReadRepository.MarkAsReadAsync(messageId, readerId);
        await chatNotificationService.NotifyMessageReadAsync(messageId, readerId);
        return read;
    }

    public async Task<IEnumerable<MessageRead>> MarkMessagesAsReadAsync(Guid chatId, Guid readerId, DateTime upToTime)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(readerId);
        var unreadMessages = await messageRepository.GetUnreadMessagesAsync(chatId, readerId);
        var messagesToMark = unreadMessages.Where(m => m.SentAt <= upToTime);
        return await messageReadRepository.MarkAsReadsAsync(messagesToMark, readerId);
    }


    public async Task<bool> DeleteMessageAsync(Guid messageId)
    {
        Guard.AgainstEmptyGuid(messageId);

        var deleted = await messageRepository.DeleteAsync(messageId);

        if (deleted)
        {
            await chatNotificationService.NotifyMessageDeletedAsync(messageId);
        }

        return deleted;
    }
}
