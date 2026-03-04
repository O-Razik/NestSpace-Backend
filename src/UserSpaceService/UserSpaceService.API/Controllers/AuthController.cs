using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.Exceptions;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.API.Controllers;

/// <summary>
/// Controller for authentication operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserService service) : ControllerBase
{
    /// <summary>
    /// Registers a new user with the provided username, email, and password.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync(
        [FromBody] RegisterDto registerDto)
    {
        var authResponse = await service.RegisterAsync(registerDto);
        return Created(new Uri(string.Empty), authResponse);
    }

    /// <summary>
    /// Logs in a user with the provided email/username and password.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto loginDto)
    {
        var authResponse = await service.LoginAsync(loginDto);
        return Ok(authResponse);
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> RefreshTokenAsync([FromBody] RefreshTokenRequestDto request)
    {
        var authResponse = await service.RefreshTokenAsync(request.RefreshToken);
        return Ok(authResponse);
    }

    /// <summary>
    /// Logs out the user by revoking the refresh token.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> LogoutAsync([FromBody] RefreshTokenRequestDto request)
    {
        await service.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    /// <summary>
    /// Registers a new user using an external provider's token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register/external")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> RegisterByExternalProviderAsync(
        [FromServices] ExternalTokenValidatorFactory factory,
        [FromBody] ExternalLoginDtoShort externalLoginDto)
    {
        var validator = factory.Create(externalLoginDto.Provider);
        var (providerUserId, email) = await validator.ValidateAsync(externalLoginDto.ProviderKey);

        if (providerUserId is null || email is null)
        {
            throw new UnauthorizedException("Invalid token.");
        }
        
        var authResponse = await service.RegisterByExternalProviderAsync(
            externalLoginDto.Provider,
            providerUserId,
            email);

        return Created(new Uri(string.Empty), authResponse);
    }

    /// <summary>
    /// Logs in a user using an external provider's token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login/external")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> LoginByExternalProviderAsync(
        [FromServices] ExternalTokenValidatorFactory factory,
        [FromBody] ExternalLoginDtoShort externalLoginDto)
    {
        var validator = factory.Create(externalLoginDto.Provider);
        var (providerUserId, _) = await validator.ValidateAsync(externalLoginDto.ProviderKey);

        if (providerUserId is null)
        {
            throw new UnauthorizedException("Invalid token.");
        }
        
        var authResponse = await service.LoginByExternalProviderAsync(
            externalLoginDto.Provider,
            providerUserId);

        return Ok(authResponse);
    }
}
