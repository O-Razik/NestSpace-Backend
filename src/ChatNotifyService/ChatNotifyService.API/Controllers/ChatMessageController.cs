using System.Security.Claims;
using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNotifyService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/space/{spaceId:Guid}/chat/{chatId:Guid}/messages")]
public class ChatMessageController(
    IMessageService messageService,
    IChatService chatService,
    IBigMapper<IMessage, MessageDto, MessageDtoShort> messageMapper)  : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MessageDto>>> GetAllMessages([FromRoute] Guid chatId)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!await chatService.IsMemberAsync(chatId, memberId))
            return Forbid("You are not a member of this chat.");
        
        var messages = await messageService.GetRecentMessagesAsync(chatId, 50);
        var result = messages.Select(msg => new MessageDto
        {
            Id = msg.Id,
            ChatId = msg.ChatId,
            SenderId = msg.SenderId,
            Content = msg.Content,
            SentAt = msg.SentAt
        }).ToList();

        return Ok(result);
    }
    
    [HttpGet("recent/{count:int}")]
    public async Task<ActionResult<List<MessageDtoShort>>> GetRecentMessages([FromRoute] Guid chatId, [FromRoute] int count = 20)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!await chatService.IsMemberAsync(chatId, memberId))
            return Forbid("You are not a member of this chat.");
        
        var messages = await messageService.GetRecentMessagesAsync(chatId, count);
        var result = messages.Select(msg => new MessageDtoShort()
        {
            Id = msg.Id,
            ChatId = msg.ChatId,
            SenderId = msg.SenderId,
            Content = msg.Content,
            SentAt = msg.SentAt
        }).ToList();

        return Ok(result);
    }
    
    [HttpGet("{messageId:guid}")]
    public async Task<ActionResult<MessageDtoShort>> GetMessage([FromRoute] Guid chatId, [FromRoute] Guid messageId)
    {
        var msg = await messageService.GetMessageByIdAsync(messageId);
        if (msg == null || msg.ChatId != chatId) 
            return NotFound();

        return Ok(new MessageDto
        {
            Id = msg.Id,
            ChatId = msg.ChatId,
            SenderId = msg.SenderId,
            Content = msg.Content,
            SentAt = msg.SentAt
        });
    }
    
    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromRoute] Guid chatId, [FromBody] CreateMessageDto createMessageDto)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!await chatService.IsMemberAsync(chatId, memberId))
            return Forbid("You are not a member of this chat.");

        var messageDto = new MessageDtoShort
        {
            ChatId = chatId,
            SenderId = memberId,
            Content = createMessageDto.Content,
            ReplyToMessageId = createMessageDto.ReplyToMessageId,
            IsDeleted = false,
            IsEdited = false,
            SentAt = DateTime.UtcNow,
            ModifiedAt = null
        };
        var createdMessage = await messageService.SendMessageAsync(messageMapper.ToEntity(messageDto));
        
        return CreatedAtAction(nameof(GetMessage), new { chatId = chatId, messageId = createdMessage.Id }, messageMapper.ToDto(createdMessage));
    }
    
    [HttpPut]
    public async Task<ActionResult<MessageDto?>> EditMessage([FromRoute] Guid chatId, [FromBody] MessageDtoShort messageDto)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!await chatService.IsMemberAsync(chatId, memberId))
            return Forbid("You are not a member of this chat.");

        var updatedMessage = await messageService.EditMessageAsync(messageMapper.ToEntity(messageDto));
        if (updatedMessage == null)
            return NotFound();
        
        return Ok(messageMapper.ToDto(updatedMessage));
    }
    
    [HttpDelete("{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid chatId, [FromRoute] Guid messageId)
    {
        var memberId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!await chatService.IsMemberAsync(chatId, memberId))
            return Forbid("You are not a member of this chat.");

        var deleted = await messageService.DeleteMessageAsync(messageId);
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
}