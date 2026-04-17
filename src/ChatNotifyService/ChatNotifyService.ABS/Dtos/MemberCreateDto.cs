using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.Dtos;

public class MemberCreateDto
{
    public Guid MemberId { get; set; }
    
    public PermissionLevel PermissionLevel { get; set; }
}
