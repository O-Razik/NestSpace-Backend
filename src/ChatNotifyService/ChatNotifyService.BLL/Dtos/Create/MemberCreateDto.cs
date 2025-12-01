using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.BLL.Dtos.Create;

public class MemberCreateDto
{
    public Guid MemberId { get; set; }
    
    public PermissionLevel PermissionLevel { get; set; }
}