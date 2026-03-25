using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceRoleRepository
{
    Task<IEnumerable<SpaceRole>> GetBySpaceAsync(Guid spaceId);
    
    Task<SpaceRole?> GetByIdAsync(Guid roleId);
    
    Task<SpaceRole> CreateAsync(Guid spaceId, string roleName, Permission permissions);
    
    Task<SpaceRole?> UpdateAsync(SpaceRole updatedRole);
    
    Task<bool> DeleteAsync(Guid roleId);
}
