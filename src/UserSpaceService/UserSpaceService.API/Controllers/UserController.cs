using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.Exceptions;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IServices;

namespace UserSpaceService.API.Controllers;

/// <summary>
/// Controller for managing user accounts, including registration, login, and external authentication.
/// </summary>
/// <param name="service"> User service for handling user operations. </param>
/// <param name="currentUser"> Helper for retrieving information about the currently authenticated user. </param>
/// <param name="userMapper"> Mapper for converting between IUser and UserDto. </param>
/// <param name="userShortMapper"> Mapper for converting between IUser and UserDtoShort. </param>
[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService service,
    IGetCurrentUser currentUser,
    IMapper<IUser,UserDto> userMapper,
    IMapper<IUser,UserDtoShort> userShortMapper) : ControllerBase
{
    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    [Authorize]
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException("User ID cannot be empty.");
        }

        var user = await service.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var response =  currentUser.UserId() == userId
            ? userMapper.ToDto(user)
            : userShortMapper.ToDto(user);

        return Ok(response);
    }

    /// <summary>
    /// Searches for a user by their email address.
    /// </summary>
    [Authorize]
    [HttpGet("search/by-email/{email}")]
    [ProducesResponseType(typeof(UserDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDtoShort>> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new BadRequestException("Email cannot be empty.");
        }

        var user = await service.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        return Ok(userShortMapper.ToDto(user));
    }
    
    /// <summary>
    /// Searches for a user by their username.
    /// </summary>
    [Authorize]
    [HttpGet("search/by-username/{username}")]
    [ProducesResponseType(typeof(UserDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDtoShort>> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new BadRequestException("Username cannot be empty.");
        }

        var user = await service.GetUserByUsernameAsync(username);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        return Ok(userShortMapper.ToDto(user));
    }
    
    /// <summary>
    /// Gets the currently authenticated user's details.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetMyself()
    {
        var userId = currentUser.UserId();
        var user = await service.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        return Ok(userMapper.ToDto(user));
    }

    /// <summary>
    /// Adds an external login to an existing user account.
    /// </summary>
    [Authorize]
    [HttpPost("{userId}/external-login/add")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> AddExternalLoginAsync(
        Guid userId,
        [FromBody] ExternalLoginDtoShort externalLoginDto)
    {
        var updatedUser = await service.AddExternalLoginAsync(
            userId,
            externalLoginDto.Provider,
            externalLoginDto.ProviderKey);

        if (updatedUser == null)
        {
            throw new NotFoundException("User not found or external login already exists.");
        }

        return Created(new Uri(string.Empty), userMapper.ToDto(updatedUser));
    }

    /// <summary>
    /// Updates an existing user with the provided user data.
    /// </summary>
    [Authorize]
    [HttpPut("{userId}/update")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateAsync([FromBody] UserDto user)
    {        
        if (user.Id == Guid.Empty)
        {
            throw new BadRequestException("User ID cannot be empty.");
        }

        if (user.Id != currentUser.UserId())
        {
            throw new ForbiddenException("You can only update your own account.");
        }
        
        var updatedUser = await service.UpdateUserAsync(userMapper.ToEntity(user));
        if (updatedUser == null)
        {
            throw new NotFoundException("User not found.");
        }

        return Ok(userMapper.ToDto(updatedUser));
    }
    
    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    [Authorize]
    [HttpDelete("{userId:guid}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(Guid userId)
    {
        if (currentUser.UserId() != userId)
        {
            throw new ForbiddenException("You can only delete your own account.");
        }

        var result = await service.DeleteUserAsync(userId);
        if (!result)
        {
            throw new NotFoundException("User not found.");
        }

        return NoContent();
    }
}
