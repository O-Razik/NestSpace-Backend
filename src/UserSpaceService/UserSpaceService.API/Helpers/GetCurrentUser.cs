using System.Security.Claims;
using UserSpaceService.ABS.Exceptions;
using UserSpaceService.ABS.IHelpers;

namespace UserSpaceService.API.Helpers;

public class GetCurrentUser(IHttpContextAccessor httpContextAccessor)
    : IGetCurrentUser
{
    private ClaimsPrincipal User => 
        httpContextAccessor.HttpContext?.User 
        ?? throw new UnauthorizedException("No user context available.");

    public Guid UserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedException("User ID claim not found in JWT token.");
        }
            
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("Invalid user ID format in JWT token.");
        }
            
        return userId;
    }

    public string? Email => User.FindFirst(ClaimTypes.Email)?.Value;

    public string? Username => User.FindFirst(ClaimTypes.Name)?.Value;

    public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;
}
