using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IServices;

public interface IChatNotificationService
{
    Task NotifyMessageSentAsync(Message message);
    
    Task NotifyMessageEditedAsync(Message message);
    
    Task NotifyMessageDeletedAsync(Guid messageId);
    
    Task NotifyMessageReadAsync(Guid messageId, Guid readerId);
    
    Task NotifyChatUpdatedAsync(Chat chat);
    
    Task NotifyChatDeletedAsync(Guid chatId);
    
    Task NotifyMemberAddedAsync(Guid chatId, ChatMember member);
    
    Task NotifyMemberRemovedAsync(Guid chatId, Guid memberId);
    
    Task NotifyChatsDeletedBySpaceIdAsync(Guid spaceId);
}
