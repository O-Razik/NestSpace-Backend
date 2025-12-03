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
/// Controller for managing solo events within a space.
/// </summary>
/// <param name="soloEventService"></param>
/// <param name="soloEventMapper"></param>
/// <param name="createMapper"></param>
/// <param name="updateMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/[controller]")]
[ApiController]
public class SoloEventController(
    ISoloEventService soloEventService,
    IMapper<ISoloEvent, SoloEventDto> soloEventMapper,
    ICreateMapper<ISoloEvent, SoloEventCreateDto> createMapper,
    ICreateMapper<ISoloEvent, SoloEventUpdateDto> updateMapper)
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
        try
        {
            var events = await soloEventService.GetSoloEventsBySpaceAsync(spaceId);
            return Ok(events.Select(soloEventMapper.ToDto));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving solo events.");
        }
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
        try
        {
            var eventItem = await soloEventService.GetSoloEventByIdAsync(eventId);
            if (eventItem == null)
                return NotFound("Solo Event not found.");
            return Ok(soloEventMapper.ToDto(eventItem));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the solo event.");
        }
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
        try
        {
            var createdEvent = await soloEventService.CreateSoloEventAsync(createMapper.ToEntity(spaceId, newEvent));
            return Created(
                "solo_event/"+ createdEvent.Id,
                soloEventMapper.ToDto(createdEvent));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the solo event.");
        }
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
    public async Task<ActionResult<SoloEventDto>> UpdateSoloEventAsync([FromRoute] Guid spaceId, [FromBody] SoloEventUpdateDto updatedEvent)
    {
        try
        {
            var result = await soloEventService.UpdateSoloEventAsync(updateMapper.ToEntity(spaceId, updatedEvent));
            if (result == null)
                return NotFound();
            return Ok(soloEventMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the solo event.");
        }
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
        try
        {
            var success = await soloEventService.DeleteSoloEventAsync(eventId);
            if (!success)
                return NotFound("Solo Event not found.");
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the solo event.");
        }
    }
}