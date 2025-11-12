using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;

namespace ChatNotifyService.BLL.Services;

public class ChatService(
    IChatRepository chatRepository,
    IChatMemberRepository memberRepository,
    IChatNotificationService notificationService) : IChatService
{
    public async Task<IEnumerable<IChat>> GetAllChatsAsync(Guid spaceId, Guid memberId)
    {
        if(spaceId == Guid.Empty)
            throw new ArgumentException("SpaceId cannot be empty", nameof(spaceId));
        
        if(memberId == Guid.Empty)
            throw new ArgumentException("MemberId cannot be empty", nameof(memberId));
        
        return await chatRepository.GetAllAsync(spaceId, memberId);
    }

    public async Task<IChat?> GetChatByIdAsync(Guid chatId, Guid memberId)
    {
        if(chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));
        
        if(memberId == Guid.Empty)
            throw new ArgumentException("MemberId cannot be empty", nameof(memberId));
        
        return await chatRepository.GetByIdAsync(chatId, memberId);
    }

    public async Task<IChat> CreateChatAsync(IChat chat)
    {
        if(chat == null)
            throw new ArgumentNullException(nameof(chat), "Chat cannot be null");
        
        var created = await chatRepository.CreateAsync(chat);
        
        await notificationService.NotifyChatUpdatedAsync(created);
        
        return created;
    }

    public async Task<IChat?> UpdateChatAsync(IChat updatedChat)
    {
        if(updatedChat == null)
            throw new ArgumentNullException(nameof(updatedChat), "Updated chat cannot be null");
        
        if(updatedChat.Id == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(updatedChat));
        
        var updated = await chatRepository.UpdateAsync(updatedChat);
        
        if(updated != null)
            await notificationService.NotifyChatUpdatedAsync(updated);
        
        return updated;
    }

    public async Task<bool> DeleteChatAsync(Guid chatId, Guid memberId)
    {
        if(chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));
        
        if(memberId == Guid.Empty)
            throw new ArgumentException("MemberId cannot be empty", nameof(memberId));
        
        var chat = await chatRepository.GetByIdAsync(chatId, memberId);
        if(chat == null)
            throw new InvalidOperationException("Chat not found or member does not have access to delete the chat");
        
        var deleted = await chatRepository.DeleteAsync(chat);
        
        if(deleted)
            await notificationService.NotifyChatDeletedAsync(chatId);
        
        return deleted;
    }

    public async Task<IEnumerable<IChatMember>> GetChatMembersAsync(Guid spaceId, Guid chatId)
    {
        if(chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));
        
        return await memberRepository.GetAllAsync(spaceId, chatId);
    }

    public async Task<IChatMember> AddMemberToChatAsync(Guid chatId, Guid memberId)
    {
        if(chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));
        
        if(memberId == Guid.Empty)
            throw new ArgumentException("MemberId cannot be empty", nameof(memberId));
        
        var member = await memberRepository.AddMemberToChatAsync(chatId, memberId);
        await notificationService.NotifyMemberAddedAsync(chatId, member);
        return member;
    }

    public async Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId)
    {
        if(chatId == Guid.Empty)
            throw new ArgumentException("ChatId cannot be empty", nameof(chatId));
        
        if(memberId == Guid.Empty)
            throw new ArgumentException("MemberId cannot be empty", nameof(memberId));
        
        var removed = await memberRepository.RemoveMemberFromChatAsync(chatId, memberId);
        if (removed)
            await notificationService.NotifyMemberRemovedAsync(chatId, memberId);

        return removed;
    }
}