using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IRepositories;

public interface IChatRepository
{
    Task<IEnumerable<IChat>> GetAllAsync(Guid spaceId, Guid memberId);
    
    Task<IChat?> GetByIdAsync(Guid chatId,  Guid memberId);
    
    Task<IChat> CreateAsync(IChat chat);
    
    Task<IChat?> UpdateAsync(IChat updatedChat);
    
    Task<bool> DeleteAsync(IChat chat);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}