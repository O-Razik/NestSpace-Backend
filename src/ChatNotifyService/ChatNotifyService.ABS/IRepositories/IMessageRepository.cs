

using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IRepositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetAllAsync(Guid chatId, int pageNumber, int pageSize);
    
    Task<IEnumerable<Message>> GetRecentMessagesAsync(Guid chatId, int count = 20);
    
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid chatId, Guid userId);
    
    Task<Message?> GetByIdAsync(Guid messageId);
    
    Task<Message> CreateAsync(Message message);
    
    Task<Message?> UpdateAsync(Message updatedMessage);
    
    Task<bool> DeleteAsync(Guid messageId);
}
