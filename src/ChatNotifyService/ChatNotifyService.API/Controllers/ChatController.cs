using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.Models;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.API.Helpers;
using ChatNotifyService.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNotifyService.API.Controllers;

/// <summary>
/// Controller for managing chat and its members.
/// </summary>
/// <param name="chatService"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/chat")]
[ApiController]
public class ChatController(
    IChatService chatService,
    GetUserHelper getUserHelper,
    IBigMapper<Chat, ChatDto, ChatDtoShort> chatMapper,
    IBigMapper<ChatMember, MemberDto, MemberDtoShort> memberMapper)
        : ControllerBase
{
    /// <summary>
    /// Gets all chats in a specific space for the authenticated member.
    /// </summary>
    /// <param name="spaceId"></param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatDtoShort>>> GetAllChats([FromRoute] Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var memberId = getUserHelper.GetCurrentUserId();
        var chats = await chatService.GetAllChatsAsync(spaceId, memberId);
        return Ok(chats.Select(chatMapper.ToShortDto));
    }
    
    /// <summary>
    /// Gets a specific chat by its ID for the authenticated member.
    /// </summary>
    /// <param name="chatId">The ID of the chat to retrieve.</param>
    [HttpGet("{chatId:guid}")]
    public async Task<ActionResult<ChatDto>> GetChatById([FromRoute] Guid chatId)
    {
        Guard.AgainstEmptyGuid(chatId);
        var memberId = getUserHelper.GetCurrentUserId();

        var chat = await chatService.GetChatByIdAsync(chatId, memberId);
        return chat == null ? NotFound() : Ok(chatMapper.ToDto(chat));
    }

    /// <summary>
    /// Creates a new chat.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="chat"></param>
    [HttpPost]
    public async Task<ActionResult<ChatDto>> CreateChat(
        [FromRoute] Guid spaceId, [FromBody] ChatCreateDto chat)
    {
        var senderGuid = getUserHelper.GetCurrentUserId();
        if (chat.Members.All(m => m.MemberId != senderGuid))
        {
            chat.Members.Add(new MemberCreateDto
            {
                MemberId = senderGuid,
                PermissionLevel = PermissionLevel.Admin
            });
        }
        else
        {
            var senderMember = chat.Members.First(m => m.MemberId == senderGuid);
            senderMember.PermissionLevel = PermissionLevel.Admin;
        }
        
        var createdChat = await chatService.CreateChatAsync(chat);
        return Created(
            new Uri("chat/" + createdChat.Id),
            chatMapper.ToDto(createdChat));
    }
    
    /// <summary>
    /// Updates an existing chat.
    /// </summary>
    /// <param name="updatedChat"></param>
    [HttpPut]
    public async Task<ActionResult<ChatDto?>> UpdateChat([FromBody] ChatDtoShort updatedChat)
    {
        Guard.AgainstNull(updatedChat);
        var memberId = getUserHelper.GetCurrentUserId();
        if(!await getUserHelper.CheckMemberPermissionInChatAsync(updatedChat.Id, PermissionLevel.Admin))
        {
            return Forbid();
        }

        var chat = await chatService.UpdateChatAsync(chatMapper.ToEntity(updatedChat));
        return chat == null ? NotFound() : Ok(chatMapper.ToDto(chat));
    }
    
    /// <summary>
    /// Deletes a chat by its ID.
    /// </summary>
    /// <param name="chatId"></param>
    [HttpDelete("{chatId:guid}")]
    public async Task<IActionResult> DeleteChat([FromRoute] Guid chatId)
    {
        Guard.AgainstEmptyGuid(chatId);
        var memberId = getUserHelper.GetCurrentUserId();
        if(!await getUserHelper.CheckMemberPermissionInChatAsync(chatId, PermissionLevel.Admin))
        {
            return Forbid();
        }
        var deleted = await chatService.DeleteChatAsync(chatId, memberId);
        return deleted ? NoContent() : NotFound();
    }
    
    /// <summary>
    /// Gets all members of a specific chat.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="chatId"></param>
    [HttpGet("{chatId:guid}/members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetChatMembers(
        [FromRoute] Guid spaceId, [FromRoute] Guid chatId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstEmptyGuid(chatId);
        var members = await chatService.GetChatMembersAsync(spaceId, chatId);
        return Ok(members.Select(memberMapper.ToDto));
    }

    /// <summary>
    /// Adds a member to a specific chat.
    /// </summary>
    /// <param name="chatId"> </param>
    /// <param name="newMember"></param>
    [HttpPost("{chatId:guid}/members")]
    public async Task<ActionResult<MemberDto?>> AddMemberToChat(
        [FromRoute] Guid chatId, [FromBody] MemberDtoShort newMember)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(newMember.MemberId);
        if (!await getUserHelper.CheckMemberPermissionInChatAsync(chatId, PermissionLevel.Admin))
        {
            return Forbid();
        }
        var member = await chatService.AddMemberToChatAsync(newMember);
        return Ok(memberMapper.ToDto(member));
    }
    
    /// <summary>
    /// Removes a member from a specific chat.
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="memberId"></param>
    [HttpDelete("{chatId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> RemoveMemberFromChat(
        [FromRoute] Guid chatId, [FromRoute] Guid memberId)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstEmptyGuid(memberId);
        
        if (!await getUserHelper.CheckMemberPermissionInChatAsync(chatId, PermissionLevel.Admin))
        {
            return Forbid();
        }
        var removed = await chatService.RemoveMemberFromChatAsync(chatId, memberId);
        return removed ? NoContent() : NotFound();
    }
}
