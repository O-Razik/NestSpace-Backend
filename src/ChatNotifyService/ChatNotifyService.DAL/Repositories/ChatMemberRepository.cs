using ChatNotifyService.ABS.Exceptions;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Repositories;

public class ChatMemberRepository(
    ChatNotifyDbContext context,
    IDateTimeProvider dateTimeProvider) : 
    IChatMemberRepository
{
    public async Task<IEnumerable<ChatMember>> GetAllAsync(Guid chatId)
    {
        var chatMembers = await context.ChatMembers
            .Where(c => c.ChatId == chatId)
            .ToListAsync();

        return chatMembers;
    }

    public async Task<ChatMember?> GetByIdAsync(Guid chatId, Guid memberId)
    {
        var chatMember = await context.ChatMembers
            .Where(c => c.ChatId == chatId && c.MemberId == memberId)
            .FirstOrDefaultAsync();

        return chatMember;
    }

    public async Task<ChatMember> AddMemberToChatAsync(ChatMember addedMember)
    {
        var newMember = addedMember;
        var exists = await context.Chats.AnyAsync(c => c.Id == newMember.ChatId);
        if (!exists)
        {
            throw new NotFoundException($"Chat with id {newMember.ChatId} not found");
        }

        newMember.JoinedAt = dateTimeProvider.UtcNow.DateTime;
        context.ChatMembers.Add(newMember);
        await context.SaveChangesAsync();

        return (await GetByIdAsync(addedMember.ChatId, addedMember.MemberId))!;
    }

    public async Task<ChatMember?> UpdateChatMemberAsync(ChatMember updatedMember)
    {
        var existingMember = await context.ChatMembers
            .FirstOrDefaultAsync(cm =>
                cm.ChatId == updatedMember.ChatId &&
                cm.MemberId == updatedMember.MemberId);

        if (existingMember == null)
        {
            throw new NotFoundException($"Chat member with ChatId {updatedMember.ChatId} and MemberId {updatedMember.MemberId} not found");
        }

        existingMember.PermissionLevel = updatedMember.PermissionLevel;

        await context.SaveChangesAsync();

        return existingMember;
    }

    
    public async Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId)
    {
        var member = await context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.ChatId == chatId && cm.MemberId == memberId);

        if (member == null)
        {
            return false;
        }

        context.ChatMembers.Remove(member);
        await context.SaveChangesAsync();
        return true;
    }
}
