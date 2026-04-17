using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class ChatRepository(ChatNotifyDbContext context) : IChatRepository
{
    public async Task<IEnumerable<Chat>> GetAllAsync(Guid spaceId, Guid memberId)
    {
        return await context.Chats
            .Include(c => c.Members)
            .Where(c =>
                c.SpaceId == spaceId &&
                c.Members.Any(m => m.MemberId == memberId))
            .ToListAsync();
    }

    public async Task<Chat?> GetByIdAsync(Guid chatId, Guid memberId)
    {
        return await context.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c =>
                c.Id == chatId &&
                c.Members.Any(m => m.MemberId == memberId));
    }

    public async Task<Chat> CreateAsync(Chat chat)
    {
        context.Chats.Add(chat);
        await context.SaveChangesAsync();
        return chat;
    }

    public async Task<Chat?> UpdateAsync(Chat updatedChat)
    {
        var existingChat = await context.Chats
            .FirstOrDefaultAsync(c => c.Id == updatedChat.Id);

        if (existingChat == null)
        {
            return null;
        }

        existingChat.Name = updatedChat.Name;
        await context.SaveChangesAsync();
        return existingChat;
    }

    public async Task<bool> DeleteAsync(Chat chat)
    {
        var existing = await context.Chats
            .FirstOrDefaultAsync(c => c.Id == chat.Id);

        if (existing == null)
        {
            return false;
        }

        context.Chats.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteBySpaceIdAsync(Guid spaceId)
    {
        var chats = await context.Chats
            .Where(c => c.SpaceId == spaceId)
            .ToListAsync();

        if (chats.Count == 0)
        {
            return false;
        }

        context.Chats.RemoveRange(chats);
        await context.SaveChangesAsync();
        return true;
    }
}
