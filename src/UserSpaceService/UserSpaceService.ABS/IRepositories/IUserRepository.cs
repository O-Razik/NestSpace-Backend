using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IRepositories;

public interface IUserRepository
{
    Task<PagedResult<User>> SearchAsync(UserFilter filter);
    
    Task<User?> GetByIdAsync(Guid userId);
    
    Task<User?> GetByUsernameAsync(string username);
    
    Task<User?> GetByEmailAsync(string email);
    
    Task<User?> GetByExternalLoginAsync(Provider provider, string providerUserId);
    
    Task<User> CreateAsync(string username, string email, string passwordHash);

    Task<User> CreateAsync(string username, string email, Provider provider, string providerUserId);

    Task<User> AddExternalLoginAsync(User user, Provider provider, string providerUserId);
    
    Task<User?> UpdateAsync(User updatedUser);
    
    Task<bool> DeleteAsync(Guid userId);
}
