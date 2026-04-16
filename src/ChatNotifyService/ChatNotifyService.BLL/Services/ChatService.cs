using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IRepositories;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.BLL.Helpers;

namespace ChatNotifyService.BLL.Services;

public class ChatService(
    IChatRepository chatRepository,
    IChatMemberRepository memberRepository,
    ICreateMapper<Chat, ChatCreateDto> createMapper,
    IBigMapper<ChatMember, MemberDto, MemberDtoShort> memberMapper,
    IChatNotificationService notificationService,
    SpaceActivityLogHelper activityLogHelper) : IChatService
{
    public async Task<IEnumerable<Chat>> GetAllChatsAsync(Guid spaceId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstEmptyGuid(memberId);
        return await chatRepository.GetAllAsync(spaceId, memberId);
    }

    public async Task<Chat?> GetChatByIdAsync(Guid chatId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(memberId);
        return await chatRepository.GetByIdAsync(chatId, memberId);
    }

    public async Task<Chat> CreateChatAsync(ChatCreateDto chat)
    {
        Guard.AgainstNull(chat);
        var created = await chatRepository.CreateAsync(createMapper.ToEntity(chat));

        await activityLogHelper.LogActivityAsync(created.SpaceId, created.Name, "ChatCreated");
        await notificationService.NotifyChatUpdatedAsync(created);
        
        return created;
    }

    public async Task<Chat?> UpdateChatAsync(Chat updatedChat)
    {
        Guard.AgainstNull(updatedChat);
        Guard.AgainstEmptyGuid(updatedChat.Id);
        
        var updated = await chatRepository.UpdateAsync(updatedChat);

        if (updated is not null)
        {
            await notificationService.NotifyChatUpdatedAsync(updated);
            await activityLogHelper.LogActivityAsync(updated.SpaceId, updated.Name, "ChatUpdated");
        }
        
        return updated;
    }

    public async Task<bool> DeleteChatAsync(Guid chatId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(memberId);
        
        var chat = await chatRepository.GetByIdAsync(chatId, memberId);
        Guard.AgainstNull(chat);
        
        var deleted = await chatRepository.DeleteAsync(chat);

        if (deleted)
        {
            await notificationService.NotifyChatDeletedAsync(chatId);
            await activityLogHelper.LogActivityAsync(chat.SpaceId, chat.Name, "ChatDeleted");
        }
        
        return deleted;
    }
    
    public async Task<bool> DeleteChatsBySpaceIdAsync(Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        
        var deleted =  await chatRepository.DeleteBySpaceIdAsync(spaceId);
        
        if(deleted)
        {
            await notificationService.NotifyChatsDeletedBySpaceIdAsync(spaceId);
            await activityLogHelper.DeleteAllLogsAsync(spaceId);
        }
        
        return deleted;
    }

    public async Task<IEnumerable<ChatMember>> GetChatMembersAsync(Guid spaceId, Guid chatId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstEmptyGuid(chatId);
        return await memberRepository.GetAllAsync(chatId);
    }

    public async Task<ChatMember?> GetChatMemberAsync(Guid chatId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(memberId);
        return await memberRepository.GetByIdAsync(chatId, memberId);
    }

    public async Task<ChatMember> AddMemberToChatAsync(MemberDtoShort chatMember)
    {
        Guard.AgainstNull(chatMember);
        Guard.AgainstEmptyGuid(chatMember.ChatId);
        Guard.AgainstEmptyGuid(chatMember.MemberId);
        var member = await memberRepository.AddMemberToChatAsync(memberMapper.ToEntity(chatMember));
        await notificationService.NotifyMemberAddedAsync(member.ChatId, member);
        return member;
    }

    public async Task<ChatMember?> UpdateChatMemberAsync(MemberDtoShort updatedChatMember)
    {
        Guard.AgainstNull(updatedChatMember);
        Guard.AgainstEmptyGuid(updatedChatMember.ChatId);
        Guard.AgainstEmptyGuid(updatedChatMember.MemberId);
        
        var updated = await memberRepository
            .UpdateChatMemberAsync(memberMapper.ToEntity(updatedChatMember));
        return updated;
    }

    public async Task<bool> RemoveMemberFromChatAsync(Guid chatId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(memberId);
        
        var removed = await memberRepository.RemoveMemberFromChatAsync(chatId, memberId);
        if (removed)
        {
            await notificationService.NotifyMemberRemovedAsync(chatId, memberId);
        }

        return removed;
    }
}
