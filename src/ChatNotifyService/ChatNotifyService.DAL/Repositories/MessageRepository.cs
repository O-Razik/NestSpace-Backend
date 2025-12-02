using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class MessageRepository(ChatNotifyDbContext dbContext) : IMessageRepository
{
    public async Task<IEnumerable<IMessage>> GetAllAsync(Guid chatId, int pageNumber, int pageSize)
    {
        return await dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<IMessage>> GetRecentMessagesAsync(Guid chatId, int count = 20)
    {
        return await dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<IMessage>> GetUnreadMessagesAsync(Guid chatId, Guid userId)
    {
        var readIds = await dbContext.MessageReads
            .Where(r => r.ReaderId == userId)
            .Select(r => r.MessageId)
            .ToListAsync();

        return await dbContext.Messages
            .Where(m => m.ChatId == chatId && !readIds.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<IMessage?> GetByIdAsync(Guid messageId)
    {
        return await dbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Reads)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<IMessage> CreateAsync(IMessage message)
    {
        var entity = (Message)message;

        await dbContext.Messages.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<IMessage?> UpdateAsync(IMessage updatedMessage)
    {
        var existing = await dbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == updatedMessage.Id);

        if (existing == null)
            return null;

        var updated = (Message)updatedMessage;

        // 👇 Оновлюємо лише дозволені поля
        existing.Content = updated.Content;
        existing.ModifiedAt = DateTime.UtcNow;
        existing.IsEdited = true;
        existing.ReplyToMessageId = updated.ReplyToMessageId;
        existing.IsDeleted = updated.IsDeleted;

        await dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid messageId)
    {
        var existing = await dbContext.Messages.FirstOrDefaultAsync(m => m.Id == messageId);

        if (existing == null)
            return false;

        dbContext.Messages.Remove(existing);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
