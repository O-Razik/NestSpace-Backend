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
/// 
/// </summary>
/// <param name="messageService"></param>
/// <param name="messageMapper"></param>
/// <param name="messageCreateMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/chat/{chatId:guid}/messages")]
[ApiController]
public class MessageController(
    IMessageService messageService,
    GetUserHelper getUserHelper,
    IBigMapper<Message, MessageDto, MessageDtoShort> messageMapper,
    ICreateMapper<Message, MessageCreateDto> messageCreateMapper
) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<MessageDtoShort>>> GetMessagesAll(
        [FromRoute] Guid chatId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstNegativeOrZero(pageNumber);
        Guard.AgainstNegativeOrZero(pageSize);
        var messages = await messageService.GetAllMessagesAsync(chatId, pageNumber, pageSize);
        var dto = messages.Select(messageMapper.ToShortDto).ToList();
        return Ok(dto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<MessageDtoShort>>> GetRecentMessages(
        [FromRoute] Guid chatId,
        [FromQuery] int count)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstNegativeOrZero(count);
        var messages = await messageService.GetRecentMessagesAsync(chatId, count);
        return Ok(messages.Select(messageMapper.ToShortDto).ToList());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<MessageDtoShort>>> GetUnreadMessages(
        [FromRoute] Guid chatId)
    {
        Guard.AgainstEmptyGuid(chatId);
        var userId = getUserHelper.GetCurrentUserId();
        var messages = await messageService.GetUnreadMessagesAsync(chatId, userId);
        return Ok(messages.Select(messageMapper.ToShortDto).ToList());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpGet("{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetMessageById(
        [FromRoute] Guid messageId)
    {
        Guard.AgainstEmptyGuid(messageId);
        var message = await messageService.GetMessageByIdAsync(messageId);
        return message == null ? NotFound() : Ok(messageMapper.ToDto(message));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage(
        [FromRoute] Guid chatId,
        [FromBody] MessageCreateDto createDto)
    {
        Guard.AgainstEmptyGuid(chatId);
        Guard.AgainstNull(createDto);
        var entity = messageCreateMapper.ToEntity(createDto);
        entity.SenderId = getUserHelper.GetCurrentUserId();

        var created = await messageService.SendMessageAsync(createDto);
        var dto = messageMapper.ToDto(created);
        return Created(new Uri("message/" + dto.Id), dto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="messageId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> EditMessage(
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        [FromBody] MessageDto updateDto)
    {
        Guard.AgainstEmptyGuid(chatId); 
        Guard.AgainstEmptyGuid(messageId); 
        Guard.AgainstNull(updateDto);
        
        updateDto.Id = messageId;
        updateDto.ChatId = chatId;
        updateDto.SenderId = getUserHelper.GetCurrentUserId();

        var updated = await messageService.EditMessageAsync(messageMapper.ToEntity(updateDto));
        return updated == null ? NotFound() : Ok(messageMapper.ToDto(updated));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpPost("{messageId:guid}/read")]
    public async Task<IActionResult> MarkMessageAsRead(
        [FromRoute] Guid messageId)
    {
        Guard.AgainstEmptyGuid(messageId);
        var userId = getUserHelper.GetCurrentUserId();
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
        Guard.AgainstEmptyGuid(chatId);
        var userId = getUserHelper.GetCurrentUserId();
        var reads = await messageService
            .MarkMessagesAsReadAsync(chatId, userId, upToTime);
        return Ok(new { Count = reads?.Count() ?? 0, UpTo = upToTime });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    [HttpDelete("{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(
        [FromRoute] Guid messageId)
    {
        Guard.AgainstEmptyGuid(messageId);
        var deleted = await messageService.DeleteMessageAsync(messageId);
        return deleted ? NoContent() : NotFound();
    }
}
