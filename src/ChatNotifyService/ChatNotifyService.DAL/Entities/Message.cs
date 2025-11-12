using ChatNotifyService.ABS.IEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatNotifyService.DAL.Entities;

public class Message : IMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ChatId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid SenderId { get; set; }

    [BsonElement("content")]
    public string Content { get; set; } = null!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SentAt { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ModifiedAt { get; set; }

    public bool IsEdited { get; set; }

    public bool IsDeleted { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid? ReplyToMessageId { get; set; }

    [BsonElement("sender")]
    public ChatMember Sender { get; set; } = null!;

    [BsonElement("reads")]
    public ICollection<MessageRead> Reads { get; set; } = new List<MessageRead>();

    [BsonIgnore]
    IChatMember IMessage.Sender
    {
        get => Sender;
        set => Sender = (ChatMember)value;
    }

    [BsonIgnore]
    ICollection<IMessageRead> IMessage.Reads
    {
        get => Reads.Cast<IMessageRead>().ToList();
        set => Reads = value.Cast<MessageRead>().ToList();
    }
}