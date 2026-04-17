using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IServices;

namespace ChatNotifyService.BLL.Helpers;

public class SpaceActivityLogHelper(
    ISpaceActivityLogService service)
{
    public async Task LogActivityAsync(Guid spaceId, string chatName,  string type)
    {
        var logEntry = new SpaceActivityLogCreateDto
        {
            SpaceId = spaceId,
            Type = type,
            Description = GetDescription(type, chatName)
        };
        
        await service.CreateActivityLogAsync(logEntry);
    }
    
    public async Task<bool> DeleteAllLogsAsync(Guid spaceId)
    {
        return await service.DeleteActivityLogsBySpaceIdAsync(spaceId);
    }
    
    private static string GetDescription(string type, string chatName)
    {
        return type switch
        {
            "ChatCreated" => $"Chat '{chatName}' created",
            "ChatDeleted" => $"Chat '{chatName}' deleted",
            "MemberAdded" => $"Member added to chat",
            "MemberRemoved" => $"Member removed from chat '{chatName}'",
            _ => "Unknown activity"
        };
    }
}
