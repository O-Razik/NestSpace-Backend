

using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IRepositories;

public interface IChatMemberRepository
{
    Task<IEnumerable<ChatMember>> GetAllAsync(Guid chatId);

    Task<ChatMember?> GetByIdAsync(Guid chatId, Guid memberId);

    Task<ChatMember> AddMemberToChatAsync(ChatMember addedMember);
    
    Task<ChatMember?> UpdateChatMemberAsync(ChatMember updatedMember);
    
    Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId);
}
