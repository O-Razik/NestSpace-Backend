using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatNotifyService.ABS.Models;

public class Chat
{
    [Key]
    [Column("chat_id")]
    public Guid Id { get; set; }

    [Column("space_id")]
    public Guid SpaceId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    public ICollection<ChatMember> Members { get; set; } = new List<ChatMember>();
}
