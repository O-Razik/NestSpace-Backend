using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using MongoDB.Driver;

namespace ChatNotifyService.DAL.Repositories;

public class ChatRepository(ChatNotifyDbContext dbContext) : IChatRepository
{
    public async Task<IEnumerable<IChat>> GetAllAsync(Guid spaceId, Guid memberId)
    {
        return await dbContext.Chats
            .Find(chat => chat.SpaceId == spaceId && chat.Members.FirstOrDefault(m => m.MemberId == memberId) != null)
            .ToListAsync();
    }

    public async Task<IChat?> GetByIdAsync(Guid chatId, Guid memberId)
    {
        return await dbContext.Chats
            .Find(chat => chat.Id == chatId && chat.Members.FirstOrDefault(m => m.MemberId == memberId) != null)
            .FirstOrDefaultAsync();
    }

    public async Task<IChat> CreateAsync(IChat chat)
    {
        await dbContext.Chats.InsertOneAsync((Chat)chat);
        return chat;
    }

    public async Task<IChat?> UpdateAsync(IChat updatedChat)
    {
        var result = await dbContext.Chats.ReplaceOneAsync(
            chat => chat.Id == updatedChat.Id,
            (Chat)updatedChat);

        if (result.IsAcknowledged && result.ModifiedCount > 0)
        {
            return updatedChat;
        }

        return null;
    }

    public async Task<bool> DeleteAsync(IChat chat)
    {
        var result = await dbContext.Chats.DeleteOneAsync(c => c.Id == chat.Id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}