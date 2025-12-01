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
/// 
/// </summary>
/// <param name="messageService"></param>
/// <param name="httpContextAccessor"></param>
/// <param name="messageMapper"></param>
/// <param name="messageCreateMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/chat/{chatId:guid}/")]
[ApiController]
public class MessageController(
    IMessageService messageService,
    IHttpContextAccessor httpContextAccessor,
    IBigMapper<IMessage, MessageDto, MessageDtoShort> messageMapper,
    ICreateMapper<IMessage, MessageCreateDto> messageCreateMapper
) : ControllerBase
{
    private Guid GetCurrentUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier) ?? user?.FindFirst("sub");
        if (idClaim == null || !Guid.TryParse(idClaim.Value, out var userId))
            throw new UnauthorizedAccessException("User id not found in claims.");
        return userId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("messages")]
    public async Task<ActionResult<IEnumerable<MessageDtoShort>>> GetMessagesAll(
        [FromRoute] Guid chatId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var messages = await messageService.GetAllMessagesAsync(chatId, pageNumber, pageSize);
        var dto = messages.Select(messageMapper.ToShortDto).ToList();
        return Ok(dto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="chatId"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet("messages/recent")]
    public async Task<ActionResult<IEnumerable<MessageDtoShort>>> GetRecentMessages(
        [FromRoute] Guid spaceId,
        [FromRoute] Guid chatId,
        [FromQuery] int count = 20)
    {
        var messages = await messageService.GetRecentMessagesAsync(chatId, count);
        return Ok(messages.Select(messageMapper.ToShortDto).ToList());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    [HttpGet("messages/unread")]
    public async Task<ActionResult<IEnumerable<MessageDtoShort>>> GetUnreadMessages(
        [FromRoute] Guid chatId)
    {
        var userId = GetCurrentUserId();
        var messages = await messageService.GetUnreadMessagesAsync(chatId, userId);
        return Ok(messages.Select(messageMapper.ToShortDto).ToList());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="chatId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpGet("message/{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetMessageById(
        [FromRoute] Guid spaceId,
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId)
    {
        var message = await messageService.GetMessageByIdAsync(messageId);
        if (message == null) return NotFound();
        return Ok(messageMapper.ToDto(message));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost("message")]
    public async Task<ActionResult<MessageDto>> SendMessage(
        [FromRoute] Guid chatId,
        [FromBody] MessageCreateDto createDto)
    {
        var entity = messageCreateMapper.ToEntity(chatId, createDto);
        entity.SenderId = GetCurrentUserId();

        var created = await messageService.SendMessageAsync(entity);
        var dto = messageMapper.ToDto(created);
        return Created("message/" + dto.Id, dto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="chatId"></param>
    /// <param name="messageId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("message/{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> EditMessage(
        [FromRoute] Guid spaceId,
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        [FromBody] MessageDto updateDto)
    {
        updateDto.Id = messageId;
        updateDto.ChatId = chatId;
        updateDto.SenderId = GetCurrentUserId();

        var updated = await messageService.EditMessageAsync(messageMapper.ToEntity(updateDto));
        if (updated == null) return NotFound();
        return Ok(messageMapper.ToDto(updated));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="chatId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpPost("message/{messageId:guid}/read")]
    public async Task<IActionResult> MarkMessageAsRead(
        [FromRoute] Guid spaceId,
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId)
    {
        var userId = GetCurrentUserId();
        await messageService.MarkMessageAsReadAsync(messageId, userId);
        return NoContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="upToTime"></param>
    /// <returns></returns>
    [HttpPost("reads")]
    public async Task<ActionResult<IEnumerable<object>>> MarkMessagesAsRead(
        [FromRoute] Guid chatId,
        [FromQuery] DateTime upToTime)
    {
        var userId = GetCurrentUserId();
        var reads = await messageService
            .MarkMessagesAsReadAsync(chatId, userId, upToTime);
        return Ok(new { Count = reads?.Count() ?? 0, UpTo = upToTime });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpDelete("message/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(
        [FromRoute] Guid messageId)
    {
        var deleted = await messageService.DeleteMessageAsync(messageId);
        if (!deleted) return NotFound();
        return NoContent();
    }
}