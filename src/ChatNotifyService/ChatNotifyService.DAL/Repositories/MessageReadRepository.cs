using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class MessageReadRepository(
    ChatNotifyDbContext context,
    IDateTimeProvider dateTimeProvider)
    : IMessageReadRepository
{
    public async Task<IEnumerable<MessageRead>> GetAllAsync(Guid messageId)
    {
        return await context.MessageReads
            .Where(r => r.MessageId == messageId)
            .Include(r => r.Reader)
            .ToListAsync();
    }

    public async Task<MessageRead?> GetByIdAsync(Guid messageId, Guid readerId)
    {
        return await context.MessageReads
            .Include(r => r.Reader)
            .FirstOrDefaultAsync(r => r.MessageId == messageId && r.ReaderId == readerId);
    }

    public async Task<bool> ExistsAsync(Guid messageId, Guid readerId)
    {
        return await context.MessageReads
            .AnyAsync(r => r.MessageId == messageId && r.ReaderId == readerId);
    }

    public async Task<MessageRead> MarkAsReadAsync(Guid messageId, Guid readerId)
    {
        // If exists → return existing record
        var existing = await GetByIdAsync(messageId, readerId);
        if (existing != null)
        {
            return existing;
        }

        // Create new record
        var messageRead = new MessageRead
        {
            MessageId = messageId,
            ReaderId = readerId,
            ReadAt = dateTimeProvider.UtcNow.DateTime
        };

        context.MessageReads.Add(messageRead);
        await context.SaveChangesAsync();

        return messageRead;
    }

    public async Task<IEnumerable<MessageRead>> MarkAsReadsAsync(IEnumerable<Message> messages, Guid readerId)
    {
        var toInsert = new List<MessageRead>();

        foreach (var messageId in messages.Select(m => m.Id))
        {
            var exists = await context.MessageReads
                .AnyAsync(r => r.MessageId == messageId && r.ReaderId == readerId);

            if (exists)
            {
                continue;
            }

            toInsert.Add(new MessageRead
            {
                MessageId = messageId,
                ReaderId = readerId,
                ReadAt = dateTimeProvider.UtcNow.DateTime
            });
        }

        if (toInsert.Count <= 0)
        {
            return toInsert;
        }
        context.MessageReads.AddRange(toInsert);
        await context.SaveChangesAsync();

        return toInsert;
    }
}
