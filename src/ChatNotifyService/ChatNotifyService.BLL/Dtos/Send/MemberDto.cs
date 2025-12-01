using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.BLL.Dtos.Send;

public class MemberDtoShort
{
    public Guid ChatId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public PermissionLevel PermissionLevel { get; set; }
}

public class MemberDto : MemberDtoShort
{
    public DateTime JoinedAt { get; set; }
}