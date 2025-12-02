using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatNotifyService.BLL.Services;

public class ChatNotificationService(IHubContext<ChatHub> hubContext) : IChatNotificationService
{
    public async Task NotifyMessageSentAsync(IMessage message)
    {
        await hubContext.Clients.Group(message.ChatId.ToString())
            .SendAsync("MessageSent", message);
    }

    public async Task NotifyMessageEditedAsync(IMessage message)
    {
        await hubContext.Clients.Group(message.ChatId.ToString())
            .SendAsync("MessageEdited", message);
    }

    public async Task NotifyMessageDeletedAsync(Guid messageId)
    {
        await hubContext.Clients.All.SendAsync("MessageDeleted", messageId);
    }

    public async Task NotifyMessageReadAsync(Guid messageId, Guid readerId)
    {
        await hubContext.Clients.All.SendAsync("MessageRead", new { messageId, readerId });
    }

    public async Task NotifyChatUpdatedAsync(IChat chat)
    {
        await hubContext.Clients.Group(chat.Id.ToString())
            .SendAsync("ChatUpdated", chat);
    }
    
    public async Task NotifyChatDeletedAsync(Guid chatId)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .SendAsync("ChatDeleted", chatId);
    }

    public async Task NotifyMemberAddedAsync(Guid chatId, IChatMember member)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .SendAsync("MemberAdded", member);
    }

    public async Task NotifyMemberRemovedAsync(Guid chatId, Guid memberId)
    {
        await hubContext.Clients.Group(chatId.ToString())
            .SendAsync("MemberRemoved", memberId);
    }

    public async Task NotifyChatsDeletedBySpaceIdAsync(Guid spaceId)
    {
        await hubContext.Clients.All
            .SendAsync("SpaceDeleted", spaceId);
    }
}