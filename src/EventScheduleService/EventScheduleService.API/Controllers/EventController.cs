using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Dto.Create;
using EventScheduleService.BLL.Dto.Send;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventScheduleService.API.Controllers;

/// <summary>
/// Controller for managing events and markers within a space.
/// </summary>
/// <param name="eventService"></param>
/// <param name="categoryMapper"></param>
/// <param name="categoryCreateMapper"></param>
/// <param name="tagMapper"></param>
/// <param name="tagCreateMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/[controller]")]
[ApiController]
public class EventController(
    IEventService eventService,
    IMapper<IEventCategory, CategoryDto, CategoryShortDto> categoryMapper,
    ICreateMapper<IEventCategory, CategoryCreateDto> categoryCreateMapper,
    IMapper<IEventTag, TagDto> tagMapper,
    ICreateMapper<IEventTag, TagCreateDto> tagCreateMapper)
    : ControllerBase
{
    /// <summary>
    /// Gets all events associated with a specific space by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryShortDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategoryShortDto>>> GetEventsBySpaceAsync(Guid spaceId)
    {
        try
        {
            var events = await eventService.GetCategoriesBySpaceAsync(spaceId);
            return Ok(events.Select(categoryMapper.ToShortDto));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving events.");
        }
    }

    /// <summary>
    /// Gets full information about an event by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="eventId"></param>
    /// <returns></returns>
    [HttpGet("categories/{eventId:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> GetEventByIdAsync([FromRoute] Guid spaceId, Guid eventId)
    {
        try
        {
            if (eventId == Guid.Empty)
                return BadRequest("Invalid event ID.");
            if (spaceId == Guid.Empty)
                return BadRequest("Invalid space ID.");
            var eventItem = await eventService.GetCategoryByIdAsync(eventId);
            if (eventItem == null)
                return NotFound("Event not found.");
            if (eventItem.SpaceId != spaceId)
                return BadRequest("Event does not belong to the specified space.");
            else return Ok(categoryMapper.ToDto(eventItem));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the event.");
        }
    }

    /// <summary>
    /// Creates a new event in the specified space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="newEvent"></param>
    /// <returns></returns>
    [HttpPost("categories")]
    [ProducesResponseType(typeof(CategoryShortDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> CreateEventAsync([FromRoute] Guid spaceId, [FromBody] CategoryCreateDto newEvent)
    {
        try
        {
            var createdEvent = await eventService.CreateCategoryAsync(categoryCreateMapper.ToEntity(spaceId, newEvent));
            return Created("catregories/" + createdEvent.Id,
                categoryMapper.ToShortDto(createdEvent));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the event.");
        }
    }

    /// <summary>
    /// Updates an existing event with the provided details.
    /// </summary>
    /// <param name="updatedEvent"></param>
    /// <returns></returns>
    [HttpPut("categories")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> UpdateEventAsync([FromBody] CategoryShortDto updatedEvent)
    {
        if (updatedEvent.Id == Guid.Empty)
            return BadRequest("Invalid event data.");

        try
        {
            var result = await eventService.UpdateCategoryAsync(categoryMapper.ToEntity(updatedEvent));
            if (result == null)
                return NotFound("Event not found.");
            return Ok(categoryMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the event.");
        }
    }

    /// <summary>
    /// Deletes an event by its ID.
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    [HttpDelete("categories/{eventId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteEventAsync(Guid eventId)
    {
        if (eventId == Guid.Empty)
            return BadRequest("Invalid event ID.");
        try
        {
            var result = await eventService.DeleteCategoryAsync(eventId);
            if (!result)
                return NotFound("Event not found.");
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the event.");
        }
    }
    
    /// <summary>
    /// Creates a new marker (event tag) in the specified space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    [HttpGet("tags")]
    [ProducesResponseType(typeof(IEnumerable<TagDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsBySpaceAsync(Guid spaceId)
    {
        try
        {
            var markers = await eventService.GetTagsBySpaceAsync(spaceId);
            return Ok(markers.Select(tagMapper.ToDto));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving markers.");
        }
    }
    
    /// <summary>
    /// Gets full information about a marker (event tag) by its ID.
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [HttpGet("tags/{markerId:guid}")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TagDto>> GetTagByIdAsync(Guid tagId)
    {
        try
        {
            var marker = await eventService.GetTagByIdAsync(tagId);
            if (marker == null)
                return NotFound("Marker not found.");
            return Ok(tagMapper.ToDto(marker));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the marker.");
        }
    }

    /// <summary>
    /// Creates a new event tag in the specified space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="newTag"></param>
    /// <returns></returns>
    [HttpPost("tags")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TagDto>> CreateTagAsync([FromRoute] Guid spaceId, [FromBody] TagCreateDto newTag)
    {
        try
        {
            var createdTag = await eventService.CreateTagAsync(tagCreateMapper.ToEntity(spaceId, newTag));
            return Created( 
                "tags/" + createdTag.Id,
                tagMapper.ToDto(createdTag));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the marker.");
        }
    }
    
    /// <summary>
    /// Updates an existing event tag with the provided details.
    /// </summary>
    /// <param name="updatedTag"></param>
    /// <returns></returns>
    [HttpPut("tags")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TagDto>> UpdateMarkerAsync([FromBody] TagDto updatedTag)
    {
        if (updatedTag.Id == Guid.Empty)
            return BadRequest("Invalid marker data.");

        try
        {
            var result = await eventService.UpdateTagAsync(tagMapper.ToEntity(updatedTag));
            if (result == null)
                return NotFound("Tag not found.");
            return Ok(tagMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the marker.");
        }
    }
    
    /// <summary>
    /// Deletes an event tag by its ID.
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [HttpDelete("tags/{markerId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteTagAsync(Guid tagId)
    {
        if (tagId == Guid.Empty)
            return BadRequest("Invalid tag ID.");
        try
        {
            var result = await eventService.DeleteTagAsync(tagId);
            if (!result)
                return NotFound("Tag not found.");
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the tag.");
        }
    }
}