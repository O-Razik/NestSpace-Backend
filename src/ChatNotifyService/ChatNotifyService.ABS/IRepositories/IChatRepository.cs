using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IRepositories;

public interface IChatRepository
{
    Task<IEnumerable<Chat>> GetAllAsync(Guid spaceId, Guid memberId);
    
    Task<Chat?> GetByIdAsync(Guid chatId,  Guid memberId);
    
    Task<Chat> CreateAsync(Chat chat);
    
    Task<Chat?> UpdateAsync(Chat updatedChat);
    
    Task<bool> DeleteAsync(Chat chat);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}
