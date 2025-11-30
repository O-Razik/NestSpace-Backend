using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Dto.Create;
using EventScheduleService.BLL.Dto.Send;
using EventScheduleService.BLL.Dto.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventScheduleService.API.Controllers;

/// <summary>
/// Controller for managing and schedules and theirs regular events within a space.
/// </summary>
/// <param name="regularEventService"></param>
/// <param name="regularEventMapper"></param>
/// <param name="createMapper"></param>
/// <param name="updateMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/[controller]")]
[ApiController]
public class RegularEventController(
    IRegularEventService regularEventService,
    IMapper<IRegularEvent, RegularEventDto> regularEventMapper,
    ICreateMapper<IRegularEvent, RegularEventCreateDto> createMapper,
    ICreateMapper<IRegularEvent, RegularEventUpdateDto> updateMapper)
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
        try
        {
            var events = await regularEventService.GetRegularEventsBySpaceAsync(spaceId);
            return Ok(events.Select(regularEventMapper.ToDto));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving regular events.");
        }
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
        try
        {
            var eventItem = await regularEventService.GetRegularEventByIdAsync(eventId);
            if (eventItem == null)
                return NotFound("Regular event not found.");
            return Ok(regularEventMapper.ToDto(eventItem));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the regular event.");
        }
    }

    /// <summary>
    /// Creates a new regular event in the specified schedule with the provided details.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="newEvent"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(RegularEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegularEventDto>> CreateRegularEventAsync([FromRoute] Guid spaceId, [FromBody] RegularEventCreateDto newEvent)
    {
        try
        {
            var createdEvent = await regularEventService.CreateRegularEventAsync(createMapper.ToEntity(spaceId, newEvent));
            return Created(
                "regular_event/" + createdEvent.Id,
                regularEventMapper.ToDto(createdEvent));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the regular event.");
        }
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
    public async Task<ActionResult<RegularEventDto>> UpdateRegularEventAsync([FromRoute] Guid spaceId, [FromBody] RegularEventUpdateDto updatedEvent)
    {
        try
        {
            var result = await regularEventService.UpdateRegularEventAsync(updateMapper.ToEntity(spaceId, updatedEvent));
            if (result == null) return NotFound();
            return Ok(regularEventMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the regular event.");
        }
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
        try
        {
            var result = await regularEventService.DeleteRegularEventAsync(eventId);
            if (!result) return NotFound("Regular event not found.");
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the regular event.");
        }
    }
}