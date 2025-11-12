using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using MongoDB.Driver;

namespace ChatNotifyService.DAL.Repositories;

public class MessageRepository(ChatNotifyDbContext dbContext) : IMessageRepository
{
    public async Task<IEnumerable<IMessage>> GetAllAsync(Guid chatId, int pageNumber, int pageSize)
    {
        return await dbContext.Messages
            .Find(message => message.ChatId == chatId)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<IMessage>> GetRecentMessagesAsync(Guid chatId, int count = 20)
    {
        return await dbContext.Messages
            .Find(message => message.ChatId == chatId)
            .SortByDescending(message => message.SentAt)
            .Limit(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<IMessage>> GetUnreadMessagesAsync(Guid chatId, Guid userId)
    {
        var readMessageIds = await dbContext.MessageReads
            .Find(read => read.ReaderId == userId)
            .Project(read => read.MessageId)
            .ToListAsync();

        return await dbContext.Messages
            .Find(message => message.ChatId == chatId && !readMessageIds.Contains(message.Id))
            .ToListAsync();
    }

    public async Task<IMessage?> GetByIdAsync(Guid messageId)
    {
        return await dbContext.Messages
            .Find(message => message.Id == messageId)
            .FirstOrDefaultAsync();
    }

    public async Task<IMessage> CreateAsync(IMessage message)
    {
        await dbContext.Messages.InsertOneAsync((Message)message);
        return message;
    }

    public async Task<IMessage?> UpdateAsync(IMessage updatedMessage)
    {
        var result = await dbContext.Messages.ReplaceOneAsync(
            message => message.Id == updatedMessage.Id,
            (Message)updatedMessage);

        if (result.IsAcknowledged && result.ModifiedCount > 0)
        {
            return updatedMessage;
        }

        return null;
    }

    public async Task<bool> DeleteAsync(Guid messageId)
    {
        var result = await dbContext.Messages.DeleteOneAsync(m => m.Id == messageId);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}