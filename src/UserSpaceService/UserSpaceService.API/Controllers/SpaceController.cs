using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Exceptions;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.API.Controllers;

/// <summary>
/// Controller for managing spaces and their members and roles.
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SpaceController(
    ISpaceService spaceService,
    IAvatarService avatarService,
    IGetCurrentUser currentUser,
    IMapper<Space, SpaceDto> spaceDtoMapper,
    IMapper<Space, SpaceDtoShort> spaceShortDtoMapper,
    IMapper<SpaceRole, SpaceRoleDto> spaceRoleDtoMapper,
    IMapper<SpaceMember, SpaceMemberDto> spaceMemberDtoMapper,
    IMapper<SpaceMember, SpaceMemberDtoShort> spaceMemberShortMapper)
    : ControllerBase
{

    /// <summary>
    /// Gets all spaces of the user based on the JWT token.
    /// </summary>
    [HttpGet("my-spaces")]
    [ProducesResponseType(typeof(IEnumerable<SpaceDtoShort>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<SpaceDtoShort>>> GetAllSpacesOfUserAsync()
    {
        var userId = currentUser.UserId();
        var result = await spaceService.GetAllSpacesOfUserAsync(userId);
        return Ok(result.Select(spaceShortDtoMapper.ToDto));
    }

    /// <summary>
    /// Gets full information about a space by its ID.
    /// </summary>
    [HttpGet("{spaceId:guid}")]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpaceDto>> GetSpaceByIdAsync([FromRoute] Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var result = await spaceService.GetSpaceByIdAsync(spaceId);
        
        if (result == null)
        {
            throw new NotFoundException("Space not found.");
        }
        
        return Ok(spaceDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Creates a new space with the specified name.
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(SpaceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SpaceDto>> CreateSpaceAsync([FromBody] CreateSpaceDto createSpaceDto)
    {
        var currentUserId = currentUser.UserId();
        
        if (createSpaceDto.CreatorId != currentUserId)
        {
            throw new ForbiddenException("You can only create spaces for yourself.");
        }
        
        var result = await spaceService.CreateSpaceAsync(createSpaceDto);
        return Created(new Uri($"spaces/{result.Id}", UriKind.Relative), spaceDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Updates the name of a space by its ID.
    /// </summary>
    [HttpPut("{spaceId:guid}/update")]
    [ProducesResponseType(typeof(SpaceDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SpaceDtoShort>> UpdateSpaceNameAsync(
        [FromRoute] Guid spaceId, 
        [FromBody] SpaceDtoShort updatedSpace)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var currentUserId = currentUser.UserId();
        
        if (string.IsNullOrWhiteSpace(updatedSpace.Name))
        {
            throw new BadRequestException("Space name cannot be null or empty.");
        }
        
        var result = await spaceService.UpdateSpaceNameAsync(spaceId, updatedSpace.Name, currentUserId);
        
        if (result == null)
        {
            throw new NotFoundException("Space not found.");
        }
        
        return Ok(spaceShortDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Deletes a space by its ID.
    /// </summary>
    [HttpDelete("{spaceId:guid}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSpaceAsync(Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var result = await spaceService.DeleteSpaceAsync(spaceId);
        
        if (!result)
        {
            throw new NotFoundException("Space not found.");
        }
        
        return NoContent();
    }

    /// <summary>
    /// Uploads or replaces the avatar of a space.
    /// </summary>
    [HttpPost("{spaceId:guid}/avatar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SpaceDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpaceDtoShort>> UploadSpaceAvatarAsync(
        [FromRoute] Guid spaceId, IFormFile avatar)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var currentUserId = currentUser.UserId();

        var space = await spaceService.GetSpaceByIdAsync(spaceId)
            ?? throw new NotFoundException("Space not found.");

        var oldAvatarUrl = space.AvatarUrl;
        var newAvatarUrl = await avatarService.SaveAvatarAsync(
            avatar.OpenReadStream(), avatar.FileName, avatar.Length, "spaces", spaceId);

        await avatarService.DeleteAvatarAsync(oldAvatarUrl);

        var updated = await spaceService.UpdateSpaceAvatarAsync(spaceId, newAvatarUrl, currentUserId)
            ?? throw new NotFoundException("Space not found.");

        return Ok(spaceShortDtoMapper.ToDto(updated));
    }

    /// <summary>
    /// Deletes the avatar of a space.
    /// </summary>
    [HttpDelete("{spaceId:guid}/avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSpaceAvatarAsync([FromRoute] Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var currentUserId = currentUser.UserId();

        var space = await spaceService.GetSpaceByIdAsync(spaceId)
            ?? throw new NotFoundException("Space not found.");

        await avatarService.DeleteAvatarAsync(space.AvatarUrl);
        await spaceService.UpdateSpaceAvatarAsync(spaceId, null, currentUserId);

        return NoContent();
    }

    /// <summary>
    /// Creates a new space role with the specified name in the given space.
    /// </summary>
    [HttpPost("{spaceId:guid}/role/create")]
    [ProducesResponseType(typeof(SpaceRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SpaceRoleDto>> CreateSpaceRoleAsync(
        [FromRoute] Guid spaceId, 
        [FromBody] SpaceRoleDtoShort role)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var currentUserId = currentUser.UserId();
        
        var result = await spaceService.CreateSpaceRoleAsync(
            spaceId, 
            role.Name, 
            role.RolePermissions, 
            currentUserId);
        
        return Created(new Uri($"spaces/{spaceId}/role/{result.Id}", UriKind.Relative), spaceRoleDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Updates an existing space role.
    /// </summary>
    [HttpPut("{spaceId:guid}/role/update")]
    [ProducesResponseType(typeof(SpaceRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SpaceRoleDto>> UpdateSpaceRoleAsync([FromBody] SpaceRoleDto spaceRole)
    {
        var currentUserId = currentUser.UserId();
        
        var result = await spaceService.UpdateSpaceRoleAsync(
            spaceRoleDtoMapper.ToEntity(spaceRole), 
            currentUserId);
        
        if (result == null)
        {
            throw new NotFoundException("Space role not found.");
        }
        
        return Ok(spaceRoleDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Deletes a space role by its ID in the specified space.
    /// </summary>
    [HttpDelete("{spaceId:guid}/role/{roleId:guid}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteSpaceRoleAsync(
        [FromRoute] Guid spaceId, 
        [FromRoute] Guid roleId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstEmptyGuid(roleId);
        var currentUserId = currentUser.UserId();
        
        var result = await spaceService.DeleteSpaceRoleAsync(spaceId, roleId, currentUserId);
        
        if (!result)
        {
            throw new NotFoundException("Space role not found.");
        }
        
        return NoContent();
    }

    /// <summary>
    /// Adds a member to a space with the specified user ID and role ID.
    /// </summary>
    [HttpPost("{spaceId:guid}/member/add")]
    [ProducesResponseType(typeof(SpaceMemberDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SpaceMemberDto>> AddMemberToSpaceAsync(
        [FromRoute] Guid spaceId, 
        [FromBody] AddSpaceMemberDto member)
    {
        Guard.AgainstEmptyGuid(spaceId);
        var result = await spaceService.AddMemberToSpaceAsync(spaceId, member.UserId, member.RoleId);
        return Created(new Uri($"space/{spaceId}/member/{result.UserId}", UriKind.Relative), spaceMemberDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Updates the information of a space member.
    /// </summary>
    [HttpPut("{spaceId:guid}/member/update")]
    [ProducesResponseType(typeof(SpaceMemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpaceMemberDto>> UpdateSpaceMemberAsync([FromBody] SpaceMemberDtoShort spaceMember)
    {
        var result = await spaceService.UpdateSpaceMemberAsync(spaceMemberShortMapper.ToEntity(spaceMember));
        
        if (result == null)
        {
            throw new NotFoundException("Space member not found.");
        }
        
        return Ok(spaceMemberDtoMapper.ToDto(result));
    }

    /// <summary>
    /// Removes a member from a space by their user ID.
    /// </summary>
    [HttpDelete("{spaceId:guid}/member/remove/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveMemberFromSpaceAsync(
        [FromRoute] Guid spaceId, [FromRoute] Guid userId)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstEmptyGuid(userId);
        var result = await spaceService.RemoveMemberFromSpaceAsync(spaceId, userId);
        
        if (!result)
        {
            throw new NotFoundException("Space member not found.");
        }
        
        return NoContent();
    }
}
