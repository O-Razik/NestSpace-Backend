using System.Security.Claims;
using ChatNotifyService.ABS.Exceptions;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.API.Helpers;

/// <summary>
/// Helper class to retrieve the current user's ID and check their permissions in a chat.
/// </summary>
/// <param name="httpContextAccessor"></param>
/// <param name="chatService"></param>
public class GetUserHelper(
    IHttpContextAccessor httpContextAccessor,
    IChatService chatService)
{
    
    /// <summary>
    /// Retrieves the current user's ID from the HTTP context claims.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public Guid GetCurrentUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier) ?? user?.FindFirst("sub");
        if (idClaim == null || !Guid.TryParse(idClaim.Value, out var userId))
        {
            throw new UnauthorizedException("User id not found in claims.");
        }

        return userId;
    }
    
    /// <summary>
    /// Checks if the current user is a member of the specified chat and retrieves their chat member information.
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task<ChatMember> CheckUserInChatAsync(Guid chatId)
    {
        Guard.AgainstEmptyGuid(chatId);
        var userId = GetCurrentUserId();
        var chatMember = await chatService.GetChatMemberAsync(chatId, userId);
        return chatMember ?? throw new UnauthorizedException("User is not a member of the chat.");
    }
    
    /// <summary>
    /// Checks if the current user has the required permission level in the specified chat.
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="requiredPermission"></param>
    /// <returns></returns>
    public async Task<bool> CheckMemberPermissionInChatAsync(Guid chatId, PermissionLevel requiredPermission)
    {
        var chatMember = await CheckUserInChatAsync(chatId);
        return chatMember.PermissionLevel >= requiredPermission;
    }
}
