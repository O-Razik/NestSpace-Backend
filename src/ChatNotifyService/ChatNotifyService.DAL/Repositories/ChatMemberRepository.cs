using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.DAL.Data;
using ChatNotifyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class ChatMemberRepository(ChatNotifyDbContext context) : IChatMemberRepository
{
    public async Task<IEnumerable<IChatMember>> GetAllAsync(Guid chatId)
    {
        var chatMembers = await context.ChatMembers
            .Where(c => c.ChatId == chatId)
            .ToListAsync();

        return chatMembers;
    }

    public async Task<IChatMember?> GetByIdAsync(Guid chatId, Guid memberId)
    {
        var chatMember = await context.ChatMembers
            .Where(c => c.ChatId == chatId && c.MemberId == memberId)
            .FirstOrDefaultAsync();

        return chatMember;
    }

    public async Task<IChatMember> AddMemberToChatAsync(IChatMember chatMember)
    {
        var newMember = (ChatMember)chatMember;
        var exists = await context.Chats.AnyAsync(c => c.Id == newMember.ChatId);
        if (!exists)
            throw new Exception("Chat not found");

        newMember.JoinedAt = DateTime.UtcNow;
        context.ChatMembers.Add(newMember);
        await context.SaveChangesAsync();

        return (await GetByIdAsync(chatMember.ChatId, chatMember.MemberId))!;
    }
    
    public async Task<IChatMember> UpdateMemberInChatAsync(IChatMember chatMember)
    {
        var updatedMember = (ChatMember)chatMember;

        var existingMember = await context.ChatMembers
            .FirstOrDefaultAsync(cm =>
                cm.ChatId == updatedMember.ChatId &&
                cm.MemberId == updatedMember.MemberId);

        if (existingMember == null)
            throw new Exception("Chat member not found");

        existingMember.PermissionLevel = updatedMember.PermissionLevel;

        await context.SaveChangesAsync();

        return existingMember;
    }

    
    public async Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId)
    {
        var member = await context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.ChatId == chatId && cm.MemberId == memberId);

        if (member == null)
            return false;

        context.ChatMembers.Remove(member);
        await context.SaveChangesAsync();
        return true;
    }
}