using System.ComponentModel.DataAnnotations.Schema;

namespace ChatNotifyService.ABS.Models;

public class ChatMember
{
    public Guid ChatId { get; set; }
    
    public Guid MemberId { get; set; }

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; }
    
    [Column("permission_level")]
    public PermissionLevel PermissionLevel { get; set; }
}

public enum PermissionLevel
{
    Read,
    Write,
    Admin
}
