using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventScheduleService.API.Controllers;

/// <summary>
/// Controller for managing events and markers within a space.
/// </summary>
/// <param name="eventService"></param>
/// <param name="categoryMapper"></param>
/// <param name="tagMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/[controller]")]
[ApiController]
public class EventController(
    IEventService eventService,
    IMapper<EventCategory, CategoryDto> categoryMapper,
    IMapper<EventCategory, CategoryShortDto> categoryShortMapper,
    IMapper<EventTag, TagDto> tagMapper)
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
    public async Task<ActionResult<IEnumerable<CategoryShortDto>>> GetEventCategoriesBySpaceAsync(Guid spaceId)
    {
        var events = await eventService.GetCategoriesBySpaceAsync(spaceId);
        return Ok(events.Select(categoryShortMapper.ToDto));
    }

    /// <summary>
    /// Gets full information about an event by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpGet("categories/{eventId:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> GetEventCategoryByIdAsync([FromRoute] Guid spaceId, Guid categoryId)
    {
        Guard.AgainstEmptyGuid(categoryId);
        Guard.AgainstEmptyGuid(spaceId);
        var eventItem = await eventService.GetCategoryByIdAsync(categoryId);
        
        return eventItem != null ?
            Ok(categoryMapper.ToDto(eventItem)) : 
            NotFound("Event not found.");
    }

    /// <summary>
    /// Creates a new event in the specified space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="newCategory"></param>
    /// <returns></returns>
    [HttpPost("categories")]
    [ProducesResponseType(typeof(CategoryShortDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> CreateCategoryAsync([FromRoute] Guid spaceId, [FromBody] CreateCategoryDto newCategory)
    {
        var createdEvent = await eventService.CreateCategoryAsync(newCategory);
        return Created(new Uri("catregories/" + createdEvent.Id),
            categoryShortMapper.ToDto(createdEvent));
    }

    /// <summary>
    /// Updates an existing event with the provided details.
    /// </summary>
    /// <param name="updatedCategory"></param>
    /// <returns></returns>
    [HttpPut("categories")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> UpdateCategoryAsync([FromBody] CategoryShortDto updatedCategory)
    {
        Guard.AgainstNull(updatedCategory);
        Guard.AgainstEmptyGuid(updatedCategory.Id);
        var result = await eventService.UpdateCategoryAsync(categoryShortMapper.ToEntity(updatedCategory));
        return result != null ? Ok(categoryMapper.ToDto(result)) : NotFound("Event not found.");
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
    public async Task<ActionResult<bool>> DeleteCategoryAsync(Guid eventId)
    {
        Guard.AgainstEmptyGuid(eventId);
        var result = await eventService.DeleteCategoryAsync(eventId);
        return result ? NoContent() : NotFound("Event not found.");
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
        var markers = await eventService.GetTagsBySpaceAsync(spaceId);
        return Ok(markers.Select(tagMapper.ToDto));
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
        var marker = await eventService.GetTagByIdAsync(tagId);
        return marker != null ? Ok(tagMapper.ToDto(marker)) : NotFound("Marker not found.");
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
    public async Task<ActionResult<TagDto>> CreateTagAsync([FromRoute] Guid spaceId, [FromBody] CreateTagDto newTag)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstNull(newTag);
        if (newTag.SpaceId != spaceId)
        {
            return BadRequest("Tag's space ID does not match the specified space ID.");
        }
        
        var createdTag = await eventService.CreateTagAsync(newTag);
        return Created( new Uri("tags/" + createdTag.Id), tagMapper.ToDto(createdTag));
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
    public async Task<ActionResult<TagDto>> UpdateTagAsync([FromBody] TagDto updatedTag)
    {
        Guard.AgainstEmptyGuid(updatedTag.Id);
        var result = await eventService.UpdateTagAsync(tagMapper.ToEntity(updatedTag));
        return result != null ? Ok(tagMapper.ToDto(result)) : NotFound("Marker not found.");
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
        Guard.AgainstEmptyGuid(tagId);
        var result = await eventService.DeleteTagAsync(tagId);
        return result ? NoContent() : NotFound("Tag not found.");
    }
}
