using Microsoft.AspNetCore.SignalR;

namespace ChatNotifyService.BLL.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChatAsync(Guid chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task LeaveChatAsync(Guid chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }
}
