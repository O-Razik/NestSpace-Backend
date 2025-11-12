using ChatNotifyService.ABS.IEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatNotifyService.DAL.Entities;

public class MessageRead : IMessageRead
{
    [BsonRepresentation(BsonType.String)]
    public Guid MessageId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ReaderId { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime ReadAt { get; set; }

    [BsonElement("reader")]
    public ChatMember Reader { get; set; } = null!;

    [BsonIgnore]
    IChatMember IMessageRead.Reader
    {
        get => Reader;
        set => Reader = (ChatMember)value;
    }
}