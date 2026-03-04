using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.API.Controllers;

/// <summary>
/// Controller for managing user accounts, including registration, login, and external authentication.
/// </summary>
/// <param name="service"> User service for handling user operations. </param>
/// <param name="httpContextAccessor"> HTTP context accessor for accessing request context. </param>
/// <param name="userMapper"> Mapper for converting between IUser and UserDto. </param>
/// <param name="userShortMapper"> Mapper for converting between IUser and UserDtoShort. </param>
[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService service,
    IHttpContextAccessor httpContextAccessor,
    IMapper<IUser,UserDto> userMapper,
    IMapper<IUser,UserDtoShort> userShortMapper) : ControllerBase
{
    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId"> User ID to retrieve. </param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> GetUser(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("User ID cannot be empty.");

        try
        {
            var user = await service.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");
            
            var senderIdClaim = httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var response = senderIdClaim == userId.ToString()
                ? userMapper.ToDto(user)
                : userShortMapper.ToDto(user);
            
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the user.");
        }
    }

    /// <summary>
    /// Searches for a user by their email address.
    /// </summary>
    /// <param name="email"> Email address of the user to search for. </param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("search/by-email/{email}")]
    [ProducesResponseType(typeof(UserDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDtoShort>> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest("Email cannot be empty.");

        try
        {
            var user = await service.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");
            
            return Ok(userShortMapper.ToDto(user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the user.");
        }
    }
    
    /// <summary>
    /// Searches for a user by their username.
    /// </summary>
    /// <param name="username"> Username of the user to search for. </param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("search/by-username/{username}")]
    [ProducesResponseType(typeof(UserDtoShort), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDtoShort>> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
            return BadRequest("Username cannot be empty.");

        try
        {
            var user = await service.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound("User not found.");
            
            return Ok(userShortMapper.ToDto(user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the user.");
        }
    }
    
    /// <summary>
    /// Gets the currently authenticated user's details.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetMyself()
    {
        try
        {
            var senderIdClaim = httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (senderIdClaim == null)
                return Unauthorized("Invalid token.");
            
            var userId = Guid.Parse(senderIdClaim);
            var user = await service.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");
            
            return Ok(userShortMapper.ToDto(user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while retrieving the user.");
        }
    }
    
    /// <summary>
    /// Registers a new user with the provided username, email, and password.
    /// </summary>
    /// <param name="registerDto"> Object containing registration details. </param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> RegisterAsync(
        [FromBody] RegisterDtoShort registerDto)
    {
        try
        {
            var jwt = await service.RegisterAsync(
                registerDto.Username,
                registerDto.Email,
                registerDto.Password);
            return Created((string?)null, jwt);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// Logs in a user with the provided email and password, returning a JWT token if successful.
    /// </summary>
    /// <param name="loginDto"> Object containing login details. </param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginDtoShort loginDto)
    {
        try
        {
            var jwt = await service.LoginAsync(loginDto.Email, loginDto.Password);
            if (jwt == null)
                return Unauthorized("Invalid email or password.");
            
            return Ok(jwt);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized("Login failed.");
        }
    }

    /// <summary>
    /// Registers a new user using an external provider's token.
    /// </summary>
    /// <param name="factory"> Object to create external token validators. </param>
    /// <param name="externalLoginDto"> Object containing external login details. </param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register/external")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> RegisterByExternalProviderAsync(
        [FromServices] ExternalTokenValidatorFactory factory,
        [FromBody] ExternalLoginDtoShort externalLoginDto)
    {
        try
        {
            var validator = factory.Create(externalLoginDto.Provider);
            var (providerUserId, email) = await validator.ValidateAsync(externalLoginDto.ProviderKey);
        
            if (providerUserId == null || email == null)
                return Unauthorized("Failed to retrieve user info from external provider.");

            var jwt = await service.RegisterByExternalProviderAsync(externalLoginDto.Provider, providerUserId, email);
            return Created((string?)null, jwt);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred during external registration.");
        }
    }
    
    /// <summary>
    /// Logs in a user using an external provider's token, returning a JWT token if successful.
    /// </summary>
    /// <param name="factory"> Object to create external token validators. </param>
    /// <param name="externalLoginDto"> Object containing external login details. </param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login/external")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> LoginByExternalProviderAsync(
        [FromServices] ExternalTokenValidatorFactory factory,
        [FromBody] ExternalLoginDtoShort externalLoginDto)
    {
        try
        {
            if (string.IsNullOrEmpty(externalLoginDto.ProviderKey))
                return BadRequest("Invalid external login data.");
            
            var validator = factory.Create(externalLoginDto.Provider);
            var (providerUserId, _) = await validator.ValidateAsync(externalLoginDto.ProviderKey);
        
            if (providerUserId == null)
                return Unauthorized("Failed to retrieve user info from external provider.");

            var jwt = await service.LoginByExternalProviderAsync(externalLoginDto.Provider, providerUserId);
            
            if (jwt == null)
                return Unauthorized("No account associated with this external login.");
            
            return Ok(jwt);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred during external login.");
        }
    }

    /// <summary>
    /// Adds an external login to an existing user account.
    /// </summary>
    /// <param name="userId"> ID of the user to add the external login to. </param>
    /// <param name="externalLoginDto"> Object containing external login details. </param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("{userId}/external-login/add")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> AddExternalLoginAsync(Guid userId,
        [FromBody] ExternalLoginDtoShort externalLoginDto)
    {
        try
        {
            var updatedUser = await service.AddExternalLoginAsync(userId, externalLoginDto.Provider, externalLoginDto.ProviderKey);
            if (updatedUser == null)
                return NotFound("User not found or external login already exists.");
            
            return Created((string?)null, userMapper.ToDto(updatedUser));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while adding external login.");
        }
    }

    /// <summary>
    /// Updates an existing user with the provided user data.
    /// </summary>
    /// <param name="user"> Updated user data. </param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("{userId}/update")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> UpdateAsync([FromBody] UserDto? user)
    {
        if (user == null)
            return BadRequest("User data cannot be null.");

        try
        {
            var updatedUser = await service.UpdateUserAsync(userMapper.ToEntity(user));
            if (updatedUser == null)
                return NotFound("User not found.");
            
            return Ok(userMapper.ToDto(updatedUser));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while updating the user.");
        }
    }
    
    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="userId"> User ID to delete. </param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("{userId:guid}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            return BadRequest("User ID cannot be empty.");

        try
        {
            var senderIdClaim = httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (senderIdClaim != userId.ToString())
                return Unauthorized("You can only delete your own account.");
            
            var result = await service.DeleteUserAsync(userId);
            if (!result)
                return NotFound("User not found.");
            
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }
}