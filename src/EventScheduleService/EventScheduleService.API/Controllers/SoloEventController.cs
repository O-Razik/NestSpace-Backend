using EventScheduleService.ABS.Dtos;
using EventScheduleService.ABS.Exceptions;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventScheduleService.API.Controllers;

/// <summary>
/// Controller for managing solo events within a space.
/// </summary>
/// <param name="soloEventService"></param>
/// <param name="soloEventMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/[controller]")]
[ApiController]
public class SoloEventController(
    ISoloEventService soloEventService,
    IMapper<SoloEvent, SoloEventDto> soloEventMapper)
    : ControllerBase
{
    /// <summary>
    /// Gets all solo events for a specific space by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<SoloEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SoloEventDto>>> GetSoloEventsBySpaceAsync(Guid spaceId)
    {
        var events = await soloEventService
            .GetSoloEventsBySpaceAsync(spaceId);
        return Ok(events.Select(soloEventMapper.ToDto));
    }

    /// <summary>
    /// Gets a full info for solo event by its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    [HttpGet("{eventId:guid}")]
    [ProducesResponseType(typeof(SoloEventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SoloEventDto>> GetSoloEventByIdAsync(Guid eventId)
    {
        var eventItem = await soloEventService.GetSoloEventByIdAsync(eventId);
        return eventItem != null
            ? Ok(soloEventMapper.ToDto(eventItem))
            : NotFound("Solo Event not found.");
    }

    /// <summary>
    /// Creates a new solo event in the specified space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="newEvent"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(SoloEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SoloEventDto>> CreateSoloEventAsync([FromRoute] Guid spaceId, [FromBody] SoloEventCreateDto newEvent)
    {
        var createdEvent = await soloEventService.CreateSoloEventAsync(newEvent);
        return Created(
            new Uri("solo_event/"+ createdEvent.Id),
            soloEventMapper.ToDto(createdEvent));
    }

    /// <summary>
    /// Updates an existing solo event with the provided data.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="updatedEvent"></param>
    /// <returns></returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(SoloEventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SoloEventDto>> UpdateSoloEventAsync([FromRoute] Guid spaceId, [FromBody] SoloEventDto updatedEvent)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstNull(updatedEvent);
        if (updatedEvent.Id != spaceId)
        {
            throw new BadRequestException("Cannot update soloEvent of other space");
        }
        var result = await soloEventService.UpdateSoloEventAsync(soloEventMapper.ToEntity(updatedEvent));
        return result == null
            ? NotFound("Solo Event not found.")
            : Ok(soloEventMapper.ToDto(result));
    }

    /// <summary>
    /// Deletes a solo event by its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    [HttpDelete("{eventId:guid}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSoloEventAsync(Guid eventId)
    {
        var success = await soloEventService.DeleteSoloEventAsync(eventId);
        return success ? NoContent() : NotFound("Solo Event not found.");
    }
}
