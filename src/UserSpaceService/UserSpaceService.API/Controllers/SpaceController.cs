using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.DTOs;

namespace UserSpaceService.API.Controllers;

/// <summary>
/// Controller for managing spaces and their members and roles.
/// </summary>
/// <param name="spaceService"></param>
/// <param name="httpContextAccessor"></param>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SpaceController(
    ISpaceService spaceService,
    IHttpContextAccessor httpContextAccessor,
    IMapper<ISpace, SpaceDto> spaceDtoMapper,
    IMapper<ISpace, SpaceDtoShort> spaceShortDtoMapper,
    IMapper<ISpaceRole, SpaceRoleDto> spaceRoleDtoMapper,
    IMapper<ISpaceMember, SpaceMemberDto> spaceMemberDtoMapper,
    IMapper<ISpaceMember, SpaceMemberDtoShort> spaceMemberShortMapper)
    : ControllerBase
{
    /// <summary>
    /// Gets all spaces of the user based on the JWT token.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpGet("my-spaces")]
    [ProducesResponseType(typeof(IEnumerable<SpaceDtoShort>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SpaceDtoShort>>> GetAllSpacesOfUserAsync()
    {
        try
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new InvalidOperationException("User ID claim not found in JWT token.");
            
            var result = await spaceService.GetAllSpacesOfUserAsync(new Guid(userIdClaim));
            return Ok(result.Select(spaceShortDtoMapper.ToDto));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }
    
    
    /// <summary>
    /// Gets full information about a space by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    [HttpGet("{spaceId:guid}")]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpaceDto>> GetSpaceByIdAsync(Guid spaceId)
    {
        try
        {
            var result = await spaceService.GetSpaceByIdAsync(spaceId);
            return Ok(spaceDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    /// <summary>
    /// Creates a new space with the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPost("create/{name}")]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SpaceDto>> CreateSpaceAsync(string name)
    {
        try
        {
            var creatorIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (creatorIdClaim == null)
                throw new InvalidOperationException("User ID claim not found in JWT token.");
            
            var result = await spaceService.CreateSpaceAsync(new Guid(creatorIdClaim), name);
            return Created((string?)null, spaceDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the space.");
        }
    }
    
    /// <summary>
    /// Updates the name of a space by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="updatedSpace"></param>
    /// <returns></returns>
    [HttpPut("{spaceId:guid}/update")]
    [ProducesResponseType(typeof(SpaceDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SpaceDtoShort>> UpdateSpaceNameAsync(Guid spaceId, [FromBody] SpaceDtoShort updatedSpace)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(updatedSpace.Name))
                return BadRequest("Invalid space data provided.");
            
            var result = await spaceService.UpdateSpaceNameAsync(spaceId, updatedSpace.Name);
            if (result == null)
                return NotFound();
            return Ok(spaceShortDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the space name.");
        }
    }
    
    /// <summary>
    /// Deletes a space by its ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <returns></returns>
    [HttpDelete("{spaceId:guid}/delete")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> DeleteSpaceAsync(Guid spaceId)
    {
        try
        {
            var result = await spaceService.DeleteSpaceAsync(spaceId);
            if (!result)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the space.");
        }
    }
    
    /// <summary>
    /// Creates a new space role with the specified name in the given space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    [HttpPost("{spaceId:guid}/role/create")]
    [ProducesResponseType(typeof(SpaceRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SpaceRoleDto>> CreateSpaceRoleAsync(Guid spaceId, [FromBody] SpaceRoleDtoShort role)
    {
        try
        {
            var result = await spaceService.CreateSpaceRoleAsync(spaceId, role.Name, role.RolePermissions);
            return Created((string?)null, spaceRoleDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the space role.");
        }
    }
    
    /// <summary>
    /// Updates an existing space role.
    /// </summary>
    /// <param name="spaceRole"></param>
    /// <returns></returns>
    [HttpPut("{spaceId:guid}/role/update")]
    [ProducesResponseType(typeof(ISpaceRole), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ISpaceRole?>> UpdateSpaceRoleAsync([FromBody] SpaceRoleDto spaceRole)
    {
        try
        {
            var result = await spaceService.UpdateSpaceRoleAsync(spaceRoleDtoMapper.ToEntity(spaceRole));
            if (result == null)
                return NotFound();
            return Ok(spaceRoleDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the space role.");
        }
    }
    
    /// <summary>
    /// Deletes a space role by its ID in the specified space.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpDelete("{spaceId:guid}/role/{roleId:guid}/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSpaceRoleAsync(Guid spaceId, Guid roleId)
    {
        try
        {
            var result = await spaceService.DeleteSpaceRoleAsync(spaceId, roleId);
            if (!result)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the space role.");
        }
    }
    
    /// <summary>
    /// Adds a member to a space with the specified user ID and role ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    [HttpPost("{spaceId:guid}/member/add")]
    [ProducesResponseType(typeof(SpaceMemberDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SpaceMemberDto>> AddMemberToSpaceAsync(Guid spaceId, [FromBody] AddSpaceMemberDto member)
    {
        try
        {
            var result = await spaceService.AddMemberToSpaceAsync(spaceId, member.UserId, member.RoleId);
            return Created((string?)null, spaceMemberDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while adding a member to the space.");
        }
    }
    
    /// <summary>
    /// Updates the information of a space member.
    /// </summary>
    /// <param name="spaceMember"></param>
    /// <returns></returns>
    [HttpPut("{spaceId:guid}/member/update")]
    [ProducesResponseType(typeof(SpaceMemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SpaceMemberDto?>> UpdateSpaceMemberAsync([FromBody] SpaceMemberDtoShort spaceMember)
    {
        try
        {
            var result = await spaceService.UpdateSpaceMemberAsync(spaceMemberShortMapper.ToEntity(spaceMember));
            if (result == null)
                return NotFound();
            return Ok(spaceMemberDtoMapper.ToDto(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the space member.");
        }
    }
    
    /// <summary>
    /// Removes a member from a space by their user ID.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete("{spaceId:guid}/member/remove/{userId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> RemoveMemberFromSpaceAsync(Guid spaceId, Guid userId)
    {
        try
        {
            var result = await spaceService.RemoveMemberFromSpaceAsync(spaceId, userId);
            if (!result)
                return NotFound();
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while removing the member from the space.");
        }
    }
}