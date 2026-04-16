using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class MessageRepository(
    ChatNotifyDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IMessageRepository
{
    public async Task<IEnumerable<Message>> GetAllAsync(Guid chatId, int pageNumber, int pageSize)
    {
        return await dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetRecentMessagesAsync(Guid chatId, int count = 20)
    {
        return await dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid chatId, Guid userId)
    {
        var readIds = await dbContext.MessageReads
            .Where(r => r.ReaderId == userId)
            .Select(r => r.MessageId)
            .ToListAsync();

        return await dbContext.Messages
            .Where(m => m.ChatId == chatId && !readIds.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<Message?> GetByIdAsync(Guid messageId)
    {
        return await dbContext.Messages
            .Include(m => m.Sender)
            .Include(m => m.Reads)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<Message> CreateAsync(Message message)
    {
        await dbContext.Messages.AddAsync(message);
        await dbContext.SaveChangesAsync();

        return message;
    }

    public async Task<Message?> UpdateAsync(Message updatedMessage)
    {
        var existing = await dbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == updatedMessage.Id);

        if (existing == null)
        {
            return null;
        }


        existing.Content = updatedMessage.Content;
        existing.ModifiedAt = dateTimeProvider.UtcNow.DateTime;
        existing.IsEdited = true;
        existing.ReplyToMessageId = updatedMessage.ReplyToMessageId;
        existing.IsDeleted = updatedMessage.IsDeleted;

        await dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid messageId)
    {
        var existing = await dbContext.Messages.FirstOrDefaultAsync(m => m.Id == messageId);

        if (existing == null)
        {
            return false;
        }

        dbContext.Messages.Remove(existing);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
