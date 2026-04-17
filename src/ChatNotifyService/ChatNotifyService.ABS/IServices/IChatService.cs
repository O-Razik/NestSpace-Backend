using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IServices;

public interface IChatService
{
    Task<IEnumerable<Chat>> GetAllChatsAsync(Guid spaceId, Guid memberId);
    
    Task<Chat?> GetChatByIdAsync(Guid chatId, Guid memberId);
    
    Task<Chat> CreateChatAsync(ChatCreateDto chat);
    
    Task<Chat?> UpdateChatAsync(Chat updatedChat);
    
    Task<bool> DeleteChatAsync(Guid chatId, Guid memberId);
    
    Task<bool> DeleteChatsBySpaceIdAsync(Guid spaceId);
    
    Task<IEnumerable<ChatMember>> GetChatMembersAsync(Guid spaceId, Guid chatId);
    
    Task<ChatMember?> GetChatMemberAsync(Guid chatId, Guid memberId);
    
    Task<ChatMember> AddMemberToChatAsync(MemberDtoShort chatMember);
    
    Task<ChatMember?> UpdateChatMemberAsync(MemberDtoShort updatedChatMember);
    
    Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId);
}
