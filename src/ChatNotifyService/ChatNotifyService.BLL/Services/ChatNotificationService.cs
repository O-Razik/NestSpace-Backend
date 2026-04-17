using ChatNotifyService.ABS.IServices;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;
using ChatNotifyService.BLL.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatNotifyService.BLL.Services;

public class ChatNotificationService(IHubContext<ChatHub> hubContext) : IChatNotificationService
{
    public async Task NotifyMessageSentAsync(Message message)
    {
        Guard.AgainstNull(message);
        await hubContext.Clients.Group(message.ChatId.ToString())
            .SendAsync("MessageSent", message);
    }

    public async Task NotifyMessageEditedAsync(Message message)
    {
        Guard.AgainstNull(message);
        await hubContext.Clients.Group(message.ChatId.ToString())
            .SendAsync("MessageEdited", message);
    }

    public async Task NotifyMessageDeletedAsync(Guid messageId)
    {
        Guard.AgainstEmptyGuid(messageId);
        await hubContext.Clients.All.SendAsync("MessageDeleted", messageId);
    }

    public async Task NotifyMessageReadAsync(Guid messageId, Guid readerId)
    {
        Guard.AgainstEmptyGuid(messageId);
        Guard.AgainstEmptyGuid(readerId);
        await hubContext.Clients.All.SendAsync("MessageRead", new { messageId, readerId });
    }

    public async Task NotifyChatUpdatedAsync(Chat chat)
    {
        Guard.AgainstNull(chat);
        await hubContext.Clients.Group(chat.Id.ToString())
            .SendAsync("ChatUpdated", chat);
    }
    
    public async Task NotifyChatDeletedAsync(Guid chatId)
    {
        Guard.AgainstEmptyGuid(chatId);
        await hubContext.Clients.Group(chatId.ToString())
            .SendAsync("ChatDeleted", chatId);
    }

    public async Task NotifyMemberAddedAsync(Guid chatId, ChatMember member)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstNull(member);
        await hubContext.Clients.Group(chatId.ToString())
            .SendAsync("MemberAdded", member);
    }

    public async Task NotifyMemberRemovedAsync(Guid chatId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(memberId);
        await hubContext.Clients.Group(chatId.ToString())
            .SendAsync("MemberRemoved", memberId);
    }

    public async Task NotifyChatsDeletedBySpaceIdAsync(Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        await hubContext.Clients.All
            .SendAsync("SpaceDeleted", spaceId);
    }
}
