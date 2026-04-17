using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IServices;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetAllMessagesAsync(Guid chatId, int pageNumber, int pageSize);
    
    Task<IEnumerable<Message>> GetRecentMessagesAsync(Guid chatId, int count = 20);
    
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid chatId, Guid userId);
    
    Task<Message?> GetMessageByIdAsync(Guid messageId);
    
    Task<Message> SendMessageAsync(MessageCreateDto message);
    
    Task<Message?> EditMessageAsync(Message updatedMessage);
    
    Task<MessageRead> MarkMessageAsReadAsync(Guid messageId, Guid readerId);
    
    Task<IEnumerable<MessageRead>> MarkMessagesAsReadAsync(Guid chatId, Guid readerId, DateTime upToTime);
    
    Task<bool> DeleteMessageAsync(Guid messageId);
}
