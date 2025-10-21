using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IServices;

public interface IUserService
{
    Task<string> RegisterAsync(string username, string email, string password);
    
    Task<string> RegisterByExternalProviderAsync(Provider provider, string providerUserId, string email);
    
    Task<string?> LoginAsync(string email, string password);
    
    Task<string?> LoginByExternalProviderAsync(Provider provider, string providerUserId);
    
    Task<IUser?> AddExternalLoginAsync(Guid userId, Provider provider, string providerUserId);
    
    Task<IUser?> GetUserByIdAsync(Guid userId);
    
    Task<IUser?> GetUserByUsernameAsync(string username);
    
    Task<IUser?> GetUserByEmailAsync(string email);
    
    Task<IUser?> UpdateUserAsync(IUser user);
    
    Task<bool> DeleteUserAsync(Guid userId);
    
    string GenerateJwtTokenAsync(IUser user, TimeSpan expiration);
}