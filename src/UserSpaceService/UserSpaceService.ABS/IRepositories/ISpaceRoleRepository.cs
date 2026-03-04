using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceRoleRepository
{
    Task<IEnumerable<ISpaceRole>> GetBySpaceAsync(Guid spaceId);
    
    Task<ISpaceRole?> GetByIdAsync(Guid roleId);
    
    Task<ISpaceRole> CreateAsync(Guid spaceId, string roleName, Permission permissions);
    
    Task<ISpaceRole?> UpdateAsync(ISpaceRole updatedRole);
    
    Task<bool> DeleteAsync(ISpaceRole role);
}
