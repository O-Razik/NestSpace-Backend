using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IServices;

public interface IChatNotificationService
{
    Task NotifyMessageSentAsync(IMessage message);
    
    Task NotifyMessageEditedAsync(IMessage message);
    
    Task NotifyMessageDeletedAsync(Guid messageId);
    
    Task NotifyMessageReadAsync(Guid messageId, Guid readerId);
    
    Task NotifyChatUpdatedAsync(IChat chat);
    
    Task NotifyChatDeletedAsync(Guid chatId);
    
    Task NotifyMemberAddedAsync(Guid chatId, IChatMember member);
    
    Task NotifyMemberRemovedAsync(Guid chatId, Guid memberId);
    
    Task NotifyChatsDeletedBySpaceIdAsync(Guid spaceId);
}