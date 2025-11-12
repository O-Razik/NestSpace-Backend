using ChatNotifyService.ABS.IEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatNotifyService.DAL.Entities;

public class Chat : IChat
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid SpaceId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("members")]
    public ICollection<ChatMember> Members { get; set; } = new List<ChatMember>();

    [BsonIgnore]
    ICollection<IChatMember> IChat.Members
    {
        get => Members.Cast<IChatMember>().ToList();
        set => Members = value.Cast<ChatMember>().ToList();
    }
}