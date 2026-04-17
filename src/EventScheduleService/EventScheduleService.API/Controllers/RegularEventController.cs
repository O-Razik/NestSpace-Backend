using EventScheduleService.ABS.Dtos;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventScheduleService.API.Controllers;

/// <summary>
/// Controller for managing and schedules and theirs regular events within a space.
/// </summary>
/// <param name="regularEventService"></param>
/// <param name="regularEventMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/[controller]")]
[ApiController]
public class RegularEventController(
    IRegularEventService regularEventService,
    IMapper<RegularEvent, RegularEventDto> regularEventMapper)
    : ControllerBase
{
    /// <summary>
    /// Gets all regular events associated with a specific space by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<RegularEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RegularEventDto>>> GetRegularEventsBySpaceAsync(Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var events = await regularEventService.GetRegularEventsBySpaceAsync(spaceId);
        return Ok(events.Select(regularEventMapper.ToDto));
    }

    /// <summary>
    /// Gets full information about a regular event by its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    [HttpGet("{eventId:guid}")]
    [ProducesResponseType(typeof(RegularEventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegularEventDto>> GetRegularEventByIdAsync(Guid eventId)
    {
        Guard.AgainstEmptyGuid(eventId);
        var eventItem = await regularEventService.GetRegularEventByIdAsync(eventId);
        return eventItem != null ? 
            Ok(regularEventMapper.ToDto(eventItem)) : 
            NotFound();
    }

    /// <summary>
    /// Creates a new regular event in the specified schedule with the provided details.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="newCategory"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(RegularEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegularEventDto>> CreateRegularEventAsync([FromRoute] Guid spaceId, [FromBody] RegularEventCreateDto newRegularEvent)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var createdEvent = await regularEventService.CreateRegularEventAsync(newRegularEvent);
        return Created(
            new Uri("regular_event/" + createdEvent.Id),
            regularEventMapper.ToDto(createdEvent));
    }

    /// <summary>
    /// Updates an existing regular event with the provided details.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="updatedEvent"></param>
    /// <returns></returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(RegularEventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegularEventDto>> UpdateRegularEventAsync([FromRoute] Guid spaceId, [FromBody] RegularEventDto updatedEvent)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstEmptyGuid(updatedEvent.Id);
        if (updatedEvent.Id != spaceId)
        {
            return BadRequest("Event ID in the body does not match the space ID in the route.");
        }
        var result = await regularEventService.UpdateRegularEventAsync(regularEventMapper.ToEntity(updatedEvent));
        return result == null
            ? NotFound("Regular event not found.")
            : Ok(regularEventMapper.ToDto(result));
    }
    
    /// <summary>
    /// Deletes a regular event by its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    [HttpDelete("{eventId:guid}/delete")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteRegularEventAsync(Guid eventId)
    {
        var result = await regularEventService.DeleteRegularEventAsync(eventId);
        return result ?
            Ok("Regular event deleted successfully.") : 
            NotFound("Regular event not found.");
    }
}
