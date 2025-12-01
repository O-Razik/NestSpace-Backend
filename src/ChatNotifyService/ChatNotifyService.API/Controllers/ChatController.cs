using System.Security.Claims;
using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos.Create;
using ChatNotifyService.BLL.Dtos.Send;
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
    IHttpContextAccessor httpContextAccessor,
    IBigMapper<IChat, ChatDto, ChatDtoShort> chatMapper,
    ICreateMapper<IChat, ChatCreateDto> chatCreateMapper,
    IBigMapper<IChatMember, MemberDto, MemberDtoShort> memberMapper,
    ICreateMapper<IChatMember, MemberCreateDto> memberCreateMapper)
        : ControllerBase
{
    /// <summary>
    /// Gets all chats in a specific space for the authenticated member.
    /// </summary>
    /// <param name="spaceId"></param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatDtoShort>>> GetAllChats([FromRoute] Guid spaceId)
    {
        var senderIdClaim = httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
        if (senderIdClaim == null)
            return Unauthorized("Invalid token.");
            
        var memberId = Guid.Parse(senderIdClaim);
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
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var chat = await chatService.GetChatByIdAsync(chatId, memberId);
        if (chat == null)
            return NotFound();
        
        return Ok(chatMapper.ToDto(chat));
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
        var newChat = chatCreateMapper.ToEntity(spaceId, chat);
        var createdChat = await chatService.CreateChatAsync(newChat);
        return Created(
            "chat/" + createdChat.Id,
            chatMapper.ToDto(createdChat));
    }
    
    /// <summary>
    /// Updates an existing chat.
    /// </summary>
    /// <param name="updatedChat"></param>
    [HttpPut]
    public async Task<ActionResult<ChatDto?>> UpdateChat([FromBody] ChatDtoShort updatedChat)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //if (!await chatService.IsMemberAsync(updatedChat.Id, memberId))
        //    return Forbid("You are not a member of this chat.");

        var chat = await chatService.UpdateChatAsync(chatMapper.ToEntity(updatedChat));
        if (chat == null)
            return NotFound();
        
        return Ok(chatMapper.ToDto(chat));
    }
    
    /// <summary>
    /// Deletes a chat by its ID.
    /// </summary>
    /// <param name="chatId"></param>
    [HttpDelete("{chatId:guid}")]
    public async Task<IActionResult> DeleteChat([FromRoute] Guid chatId)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var deleted = await chatService.DeleteChatAsync(chatId, memberId);
        if (!deleted)
            return NotFound();
        
        return NoContent();
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
        [FromRoute] Guid chatId, [FromBody] MemberCreateDto newMember)
    {
        var member = await chatService.AddMemberToChatAsync(memberCreateMapper.ToEntity(chatId, newMember));
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
        var removed = await chatService.RemoveMemberFromChatAsync(chatId, memberId);
        if (!removed)
            return NotFound();
        
        return NoContent();
    }
}