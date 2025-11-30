using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChatNotifyService.ABS.IEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatNotifyService.DAL.Entities;

public class Chat : IChat
{
    [Key]
    [Column("chat_id")]
    public Guid Id { get; set; }

    [Column("space_id")]
    public Guid SpaceId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    public ICollection<ChatMember> Members { get; set; } = new List<ChatMember>();

    [BsonIgnore]
    ICollection<IChatMember> IChat.Members
    {
        get => Members.Cast<IChatMember>().ToList();
        set => Members = value.Cast<ChatMember>().ToList();
    }
}