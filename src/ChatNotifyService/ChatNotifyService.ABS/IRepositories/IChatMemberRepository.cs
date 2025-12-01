using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IRepositories;

public interface IChatMemberRepository
{
    Task<IEnumerable<IChatMember>> GetAllAsync(Guid chatId);

    Task<IChatMember?> GetByIdAsync(Guid chatId, Guid memberId);

    Task<IChatMember> AddMemberToChatAsync(IChatMember chatMember);
    
    Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId);
}