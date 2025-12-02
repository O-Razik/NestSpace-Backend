using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class ChatRepository(ChatNotifyDbContext context) : IChatRepository
{
    public async Task<IEnumerable<IChat>> GetAllAsync(Guid spaceId, Guid memberId)
    {
        return await context.Chats
            .Include(c => c.Members)
            .Where(c =>
                c.SpaceId == spaceId &&
                c.Members.Any(m => m.MemberId == memberId))
            .ToListAsync();
    }

    public async Task<IChat?> GetByIdAsync(Guid chatId, Guid memberId)
    {
        return await context.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c =>
                c.Id == chatId &&
                c.Members.Any(m => m.MemberId == memberId));
    }

    public async Task<IChat> CreateAsync(IChat chat)
    {
        var entity = (Chat)chat;
        context.Chats.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<IChat?> UpdateAsync(IChat chat)
    {
        var updatedChat = (Chat)chat;

        var existingChat = await context.Chats
            .FirstOrDefaultAsync(c => c.Id == updatedChat.Id);

        if (existingChat == null)
            return null;

        existingChat.Name = updatedChat.Name;
        await context.SaveChangesAsync();
        return existingChat;
    }

    public async Task<bool> DeleteAsync(IChat chat)
    {
        var entity = (Chat)chat;

        var existing = await context.Chats
            .FirstOrDefaultAsync(c => c.Id == entity.Id);

        if (existing == null)
            return false;

        context.Chats.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
