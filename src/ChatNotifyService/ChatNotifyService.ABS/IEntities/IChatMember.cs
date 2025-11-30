namespace ChatNotifyService.ABS.IEntities;

public enum PermissionLevel
{
    Read,
    Write,
    Admin
}

public interface IChatMember
{
    Guid ChatId { get; set; }
    
    Guid MemberId { get; set; }
    
    DateTime JoinedAt { get; set; }
    
    PermissionLevel PermissionLevel { get; set; }
}