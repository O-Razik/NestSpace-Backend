using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IServices;

public interface ISpaceService
{
    Task<PagedResult<ISpace>> SearchSpacesAsync(SpaceFilter filter);
    
    Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId);
    
    Task<ISpace> GetSpaceByIdAsync(Guid spaceId);
    
    Task<ISpace> CreateSpaceAsync(Guid creatorId, string name, List<Guid> memberIds);
    
    Task<ISpace?> UpdateSpaceNameAsync(Guid spaceId, string newName);
    
    Task<bool> DeleteSpaceAsync(Guid spaceId);
    
    Task<ISpaceRole> CreateSpaceRoleAsync(Guid spaceId, string roleName, Permission permissions);
    
    Task<ISpaceRole?> UpdateSpaceRoleAsync(ISpaceRole spaceRole);
    
    Task<bool> DeleteSpaceRoleAsync(Guid spaceId, Guid roleId);
    
    Task<ISpaceMember> AddMemberToSpaceAsync(Guid spaceId, Guid userId, Guid roleId);
    
    Task<ISpaceMember?> UpdateSpaceMemberAsync(ISpaceMember spaceMember);
    
    Task<bool> RemoveMemberFromSpaceAsync(Guid spaceId, Guid userId);
}