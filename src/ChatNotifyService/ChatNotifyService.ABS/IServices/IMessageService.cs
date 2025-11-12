using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IServices;

public interface IMessageService
{
    Task<IEnumerable<IMessage>> GetAllMessagesAsync(Guid chatId, int pageNumber, int pageSize);
    
    Task<IEnumerable<IMessage>> GetRecentMessagesAsync(Guid chatId, int count = 20);
    
    Task<IEnumerable<IMessage>> GetUnreadMessagesAsync(Guid chatId, Guid userId);
    
    Task<IMessage?> GetMessageByIdAsync(Guid messageId);
    
    Task<IMessage> SendMessageAsync(IMessage message);
    
    Task<IMessage?> EditMessageAsync(IMessage updatedMessage);
    
    Task<IMessageRead> MarkMessageAsReadAsync(Guid messageId, Guid readerId);
    
    Task<IEnumerable<IMessageRead>> MarkMessagesAsReadAsync(Guid chatId, Guid readerId, DateTime upToTime);
    
    Task<bool> DeleteMessageAsync(Guid messageId);
}