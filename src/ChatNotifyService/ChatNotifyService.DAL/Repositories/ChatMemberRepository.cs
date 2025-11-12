using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using MongoDB.Driver;

namespace ChatNotifyService.DAL.Repositories;

public class ChatMemberRepository(ChatNotifyDbContext dbContext) : IChatMemberRepository
{
    public async Task<IEnumerable<IChatMember>> GetAllAsync(Guid spaceId, Guid chatId)
    {
        var chat = await dbContext.Chats
            .Find(c => c.SpaceId == spaceId && c.Id == chatId)
            .FirstOrDefaultAsync();

        return chat?.Members ?? Enumerable.Empty<IChatMember>();
    }

    public async Task<IChatMember?> GetByIdAsync(Guid chatId, Guid memberId)
    {
        var chat = await dbContext.Chats
            .Find(c => c.Id == chatId)
            .FirstOrDefaultAsync();

        return chat?.Members.FirstOrDefault(m => m.MemberId == memberId);
    }

    public async Task<IChatMember> AddMemberToChatAsync(Guid chatId, Guid memberId)
    {
        var chat = await dbContext.Chats
            .Find(c => c.Id == chatId)
            .FirstOrDefaultAsync();

        if (chat == null)
        {
            throw new Exception("Chat not found");
        }

        var newMember = new ChatMember
        {
            MemberId = memberId,
            JoinedAt = DateTime.UtcNow
        };

        chat.Members.Add(newMember);

        await dbContext.Chats.ReplaceOneAsync(c => c.Id == chatId, chat);

        return newMember;
    }
    
    public async Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId)
    {
        var chat = await dbContext.Chats
            .Find(c => c.Id == chatId)
            .FirstOrDefaultAsync();

        var memberToRemove = chat?.Members.FirstOrDefault(m => m.MemberId == memberId);
        if (memberToRemove == null)
        {
            return false;
        }

        chat!.Members.Remove(memberToRemove);

        var result = await dbContext.Chats.ReplaceOneAsync(c => c.Id == chatId, chat);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}