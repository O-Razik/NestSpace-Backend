using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IServices;

public interface IChatService
{
    Task<IEnumerable<IChat>> GetAllChatsAsync(Guid spaceId, Guid memberId);
    
    Task<IChat?> GetChatByIdAsync(Guid chatId, Guid memberId);
    
    Task<IChat> CreateChatAsync(IChat chat);
    
    Task<IChat?> UpdateChatAsync(IChat updatedChat);
    
    Task<bool> DeleteChatAsync(Guid chatId, Guid memberId);
    
    Task<IEnumerable<IChatMember>> GetChatMembersAsync(Guid spaceId, Guid chatId);
    
    Task<IChatMember> AddMemberToChatAsync(Guid chatId, Guid memberId);
    
    Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId);

    Task<bool> IsMemberAsync(Guid chatId, Guid userId);
}