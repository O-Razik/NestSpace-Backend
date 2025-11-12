using System.Security.Claims;
using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ChatNotifyService.API.Controllers;

/// <summary>
/// Controller for managing chat and its members.
/// </summary>
/// <param name="chatService"></param>
[Route("api/[controller]")]
[ApiController]
public class ChatController(
    IChatService chatService,
    IBigMapper<IChat, ChatDto, ChatDtoShort> chatMapper,
    IBigMapper<IChatMember, ChatMemberDto, ChatMemberDtoShort> memberMapper) : ControllerBase
{
    /// <summary>
    /// Gets all chats in a specific space for the authenticated member.
    /// </summary>
    /// <param name="spaceId"></param>
    [HttpGet("space/{spaceId}")]
    public async Task<ActionResult<IEnumerable<ChatDtoShort>>> GetAllChats(Guid spaceId)
    {
        var memberId = Guid.Parse("b38d964b-813b-4294-8176-0a897b67b65d"); //Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var chats = await chatService.GetAllChatsAsync(spaceId, memberId);
        return Ok(chats.Select(chatMapper.ToShortDto));
    }
    
    /// <summary>
    /// Gets a specific chat by its ID for the authenticated member.
    /// </summary>
    /// <param name="chatId">The ID of the chat to retrieve.</param>
    [HttpGet("{chatId}")]
    public async Task<ActionResult<ChatDto>> GetChatById(Guid chatId)
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
    /// <param name="chat"></param>
    [HttpPost]
    public async Task<ActionResult<ChatDto>> CreateChat([FromBody] ChatDto chat)
    {
        var createdChat = await chatService.CreateChatAsync(chatMapper.ToEntity(chat));
        return CreatedAtAction(nameof(GetChatById), new { chatId = createdChat.Id }, chatMapper.ToDto(createdChat));
    }
    
    /// <summary>
    /// Updates an existing chat.
    /// </summary>
    /// <param name="updatedChat"></param>
    [HttpPut]
    public async Task<ActionResult<ChatDto?>> UpdateChat([FromBody] ChatDtoShort updatedChat)
    {
        var chat = await chatService.UpdateChatAsync(chatMapper.ToEntity(updatedChat));
        if (chat == null)
            return NotFound();
        
        return Ok(chatMapper.ToDto(chat));
    }
    
    /// <summary>
    /// Deletes a chat by its ID.
    /// </summary>
    /// <param name="chatId"></param>
    [HttpDelete("{chatId}")]
    public async Task<IActionResult> DeleteChat(Guid chatId)
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
    [HttpGet("space/{spaceId}/chat/{chatId}/members")]
    public async Task<ActionResult<IEnumerable<ChatMemberDto>>> GetChatMembers(Guid spaceId, Guid chatId)
    {
        var members = await chatService.GetChatMembersAsync(spaceId, chatId);
        return Ok(members.Select(memberMapper.ToDto));
    }
    
    /// <summary>
    /// Adds a member to a specific chat.
    /// </summary>
    /// <param name="chatId"> </param>
    /// <param name="memberId"></param>
    [HttpPost("{chatId}/members/{memberId}")]
    public async Task<ActionResult<ChatMemberDto?>> AddMemberToChat(Guid chatId, Guid memberId)
    {
        var member = await chatService.AddMemberToChatAsync(chatId, memberId);
        return Ok(memberMapper.ToDto(member));
    }
    
    /// <summary>
    /// Removes a member from a specific chat.
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="memberId"></param>
    [HttpDelete("{chatId}/members/{memberId}")]
    public async Task<IActionResult> RemoveMemberFromChat(Guid chatId, Guid memberId)
    {
        var removed = await chatService.RemoveMemberFromChatAsync(chatId, memberId);
        if (!removed)
            return NotFound();
        
        return NoContent();
    }
}