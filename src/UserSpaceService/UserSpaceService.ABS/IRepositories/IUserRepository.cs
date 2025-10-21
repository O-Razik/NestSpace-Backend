using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IRepositories;

public interface IUserRepository
{
    Task<IEnumerable<IUser>> GetAllAsync();
    
    Task<IUser?> GetByIdAsync(Guid userId);
    
    Task<IUser?> GetByUsernameAsync(string username);
    
    Task<IUser?> GetByEmailAsync(string email);
    
    Task<IUser?> GetByExternalLoginAsync(Provider provider, string providerUserId);
    
    Task<IUser> CreateAsync(string username, string email, string passwordHash);

    Task<IUser> CreateAsync(string username, string email, Provider provider, string providerUserId);

    Task<IUser> AddExternalLoginAsync(IUser user, Provider provider, string providerUserId);
    
    Task<IUser?> UpdateAsync(IUser updatedUser);
    
    Task<bool> DeleteAsync(IUser user);
}