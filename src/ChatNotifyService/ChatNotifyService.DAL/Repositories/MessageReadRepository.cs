using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class MessageReadRepository(ChatNotifyDbContext context) : IMessageReadRepository
{
    public async Task<IEnumerable<IMessageRead>> GetAllAsync(Guid messageId)
    {
        return await context.MessageReads
            .Where(r => r.MessageId == messageId)
            .Include(r => r.Reader)
            .ToListAsync();
    }

    public async Task<IMessageRead?> GetByIdAsync(Guid messageId, Guid readerId)
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

    public async Task<IMessageRead> MarkAsReadAsync(Guid messageId, Guid readerId)
    {
        // If exists → return existing record
        var existing = await GetByIdAsync(messageId, readerId);
        if (existing != null)
            return existing;

        // Create new record
        var messageRead = new MessageRead
        {
            MessageId = messageId,
            ReaderId = readerId,
            ReadAt = DateTime.UtcNow
        };

        context.MessageReads.Add(messageRead);
        await context.SaveChangesAsync();

        return messageRead;
    }

    public async Task<IEnumerable<IMessageRead>> MarkAsReadsAsync(IEnumerable<IMessage> messages, Guid readerId)
    {
        var toInsert = new List<MessageRead>();

        foreach (var message in messages)
        {
            var exists = await context.MessageReads
                .AnyAsync(r => r.MessageId == message.Id && r.ReaderId == readerId);

            if (exists) continue;

            toInsert.Add(new MessageRead
            {
                MessageId = message.Id,
                ReaderId = readerId,
                ReadAt = DateTime.UtcNow
            });
        }

        if (toInsert.Count <= 0) return toInsert;
        context.MessageReads.AddRange(toInsert);
        await context.SaveChangesAsync();

        return toInsert;
    }
}
