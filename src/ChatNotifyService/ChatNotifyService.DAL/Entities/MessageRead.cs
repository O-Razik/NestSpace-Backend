using System.ComponentModel.DataAnnotations.Schema;
using ChatNotifyService.ABS.IEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatNotifyService.DAL.Entities;

public class MessageRead : IMessageRead
{
    [Column("message_id")]
    public Guid MessageId { get; set; }

    [Column("reader_id")]
    public Guid ReaderId { get; set; }

    [Column("read_at")]
    public DateTime ReadAt { get; set; }

    [ForeignKey("ReaderId")]
    public ChatMember Reader { get; set; } = null!;

    IChatMember IMessageRead.Reader
    {
        get => Reader;
        set => Reader = (ChatMember)value;
    }
}