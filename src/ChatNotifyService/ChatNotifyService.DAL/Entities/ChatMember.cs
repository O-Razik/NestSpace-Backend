using System.ComponentModel.DataAnnotations.Schema;
using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.DAL.Entities;

public class ChatMember : IChatMember
{
    public Guid ChatId { get; set; }
    
    public Guid MemberId { get; set; }

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; }
    
    [Column("permission_level")]
    public PermissionLevel PermissionLevel { get; set; }
}