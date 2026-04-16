using System.ComponentModel.DataAnnotations.Schema;

namespace ChatNotifyService.ABS.Models;

public class MessageRead
{
    [Column("message_id")]
    public Guid MessageId { get; set; }
    
    [Column("chat_id")]
    public Guid ChatId { get; set; }

    [Column("reader_id")]
    public Guid ReaderId { get; set; }

    [Column("read_at")]
    public DateTime ReadAt { get; set; }

    [ForeignKey("ReaderId")]
    public ChatMember Reader { get; set; } = null!;

    [ForeignKey("MessageId")]
    public Message Message { get; set; } = null!;
}
