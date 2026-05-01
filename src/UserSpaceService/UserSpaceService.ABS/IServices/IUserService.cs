using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IServices;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    
    Task<AuthResponseDto> RegisterByExternalProviderAsync(Provider provider, string providerUserId, string email);
    
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    
    Task<AuthResponseDto?> LoginByExternalProviderAsync(Provider provider, string providerUserId);
    
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    
    Task LogoutAsync(string refreshToken);

    Task<User?> AddExternalLoginAsync(Guid userId, Provider provider, string providerUserId);

    Task<PagedResult<User>> SearchUsersAsync(UserFilter filter);

    Task<User?> GetUserByIdAsync(Guid userId);

    Task<User?> GetUserByUsernameAsync(string username);

    Task<User?> GetUserByEmailAsync(string email);

    Task<User?> UpdateUserAsync(User user);

    Task<User?> UpdateUserAvatarAsync(Guid userId, string? avatarUrl);

    Task<bool> DeleteUserAsync(Guid userId);
}
