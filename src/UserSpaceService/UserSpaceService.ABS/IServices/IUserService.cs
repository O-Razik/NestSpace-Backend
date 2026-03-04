using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IServices;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    
    Task<AuthResponseDto> RegisterByExternalProviderAsync(Provider provider, string providerUserId, string email);
    
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    
    Task<AuthResponseDto?> LoginByExternalProviderAsync(Provider provider, string providerUserId);
    
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    
    Task LogoutAsync(string refreshToken);

    Task<IUser?> AddExternalLoginAsync(Guid userId, Provider provider, string providerUserId);

    Task<PagedResult<IUser>> SearchUsersAsync(UserFilter filter);

    Task<IUser?> GetUserByIdAsync(Guid userId);

    Task<IUser?> GetUserByUsernameAsync(string username);

    Task<IUser?> GetUserByEmailAsync(string email);

    Task<IUser?> UpdateUserAsync(IUser user);

    Task<bool> DeleteUserAsync(Guid userId);
}
