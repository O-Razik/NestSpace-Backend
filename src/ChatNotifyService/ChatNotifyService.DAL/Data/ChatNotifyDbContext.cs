using ChatNotifyService.DAL.Entities;
using MongoDB.Driver;

namespace ChatNotifyService.DAL.Data;

public class ChatNotifyDbContext
{
    private readonly IMongoDatabase _database;
    
    public ChatNotifyDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        this._database = client.GetDatabase(databaseName);
    }
    
    public IMongoCollection<Chat> Chats => 
        _database.GetCollection<Chat>("Chats");
    
    public IMongoCollection<Message> Messages => 
        _database.GetCollection<Message>("Messages");
    
    public IMongoCollection<ChatMember> ChatMembers => 
        _database.GetCollection<ChatMember>("ChatMembers");
    
    public IMongoCollection<MessageRead> MessageReads => 
        _database.GetCollection<MessageRead>("MessageReads");
    
    public async Task InitializeIndexesAsync()
    {
        var chatIndexKeys = Builders<Chat>.IndexKeys.Ascending(c => c.SpaceId);
        await Chats.Indexes.CreateOneAsync(new CreateIndexModel<Chat>(chatIndexKeys));

        var messageIndexKeys = Builders<Message>.IndexKeys
            .Ascending(m => m.ChatId)
            .Ascending(m => m.SentAt);
        await Messages.Indexes.CreateOneAsync(new CreateIndexModel<Message>(messageIndexKeys));

        var memberIndexKeys = Builders<ChatMember>.IndexKeys
            .Ascending(cm => cm.ChatId)
            .Ascending(cm => cm.MemberId);
        await ChatMembers.Indexes.CreateOneAsync(new CreateIndexModel<ChatMember>(memberIndexKeys));
    }
}