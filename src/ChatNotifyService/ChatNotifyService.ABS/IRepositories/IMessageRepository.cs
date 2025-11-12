using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IRepositories;

public interface IMessageRepository
{
    Task<IEnumerable<IMessage>> GetAllAsync(Guid chatId, int pageNumber, int pageSize);
    
    Task<IEnumerable<IMessage>> GetRecentMessagesAsync(Guid chatId, int count = 20);
    
    Task<IEnumerable<IMessage>> GetUnreadMessagesAsync(Guid chatId, Guid userId);
    
    Task<IMessage?> GetByIdAsync(Guid messageId);
    
    Task<IMessage> CreateAsync(IMessage message);
    
    Task<IMessage?> UpdateAsync(IMessage updatedMessage);
    
    Task<bool> DeleteAsync(Guid messageId);
}