using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.DAL.Entities;

public class Message : IMessage
{
    [Key]
    [Column("message_id")]
    public Guid Id { get; set; }

    [Column("chat_id")]
    public Guid ChatId { get; set; }

    [Column("sender_id")]
    public Guid SenderId { get; set; }

    [Column("content")]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Column("timestamp")]
    public DateTime SentAt { get; set; }

    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("is_edited")]
    public bool IsEdited { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("reply_to_message_id")]
    public Guid? ReplyToMessageId { get; set; }

    [ForeignKey("SenderId")]
    public ChatMember Sender { get; set; } = null!;
    
    public ICollection<MessageRead> Reads { get; set; } = new List<MessageRead>();

    IChatMember IMessage.Sender
    {
        get => Sender;
        set => Sender = (ChatMember)value;
    }

    ICollection<IMessageRead> IMessage.Reads
    {
        get => Reads.Cast<IMessageRead>().ToList();
        set => Reads = value.Cast<MessageRead>().ToList();
    }
}