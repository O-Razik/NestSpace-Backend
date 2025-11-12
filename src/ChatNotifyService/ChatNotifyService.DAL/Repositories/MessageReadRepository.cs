using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using MongoDB.Driver;

namespace ChatNotifyService.DAL.Repositories;

public class MessageReadRepository(ChatNotifyDbContext dbContext) : IMessageReadRepository
{
    public async Task<IEnumerable<IMessageRead>> GetAllAsync(Guid messageId)
    {
        return await dbContext.MessageReads
            .Find(read => read.MessageId == messageId)
            .ToListAsync();
    }

    public async Task<IMessageRead?> GetByIdAsync(Guid messageId, Guid readerId)
    {
        return await dbContext.MessageReads
            .Find(read => read.MessageId == messageId && read.ReaderId == readerId)
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> ExistsAsync(Guid messageId, Guid readerId)
    {
        var count = await dbContext.MessageReads
            .CountDocumentsAsync(read => read.MessageId == messageId && read.ReaderId == readerId);

        return count > 0;
    }

    public async Task<IMessageRead> MarkAsReadAsync(Guid messageId, Guid readerId)
    {
        if (await ExistsAsync(messageId, readerId))
        {
            return await GetByIdAsync(messageId, readerId) 
                   ?? throw new InvalidOperationException("Message read entry exists but cannot be retrieved.");
        }
        
        var messageRead = new MessageRead
        {
            MessageId = messageId,
            ReaderId = readerId,
            ReadAt = DateTime.UtcNow
        };

        await dbContext.MessageReads.InsertOneAsync(messageRead);
        return messageRead;
    }
    
    public async Task<IEnumerable<IMessageRead>> MarkAsReadsAsync(IEnumerable<IMessage> messages, Guid readerId)
    {
        var messageReads = new List<IMessageRead>();

        foreach (var message in messages)
        {
            if (await ExistsAsync(message.Id, readerId)) continue;
            var messageRead = new MessageRead
            {
                MessageId = message.Id,
                ReaderId = readerId,
                ReadAt = DateTime.UtcNow
            };

            messageReads.Add(messageRead);
        }

        if (messageReads.Count != 0)
        {
            await dbContext.MessageReads.InsertManyAsync(messageReads.Cast<MessageRead>());
        }

        return messageReads;
    }
}