using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IServices;

public interface ISpaceService
{
    Task<PagedResult<ISpace>> SearchSpacesAsync(SpaceFilter filter);
    
    Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId);
    
    Task<ISpace?> GetSpaceByIdAsync(Guid spaceId);
    
    Task<ISpace> CreateSpaceAsync(CreateSpaceDto createSpaceDto);
    
    Task<ISpace?> UpdateSpaceNameAsync(Guid spaceId, string newName, Guid memberId);
    
    Task<bool> DeleteSpaceAsync(Guid spaceId);
    
    Task<ISpaceRole> CreateSpaceRoleAsync(Guid spaceId, string roleName, Permission permissions, Guid memberId);
    
    Task<ISpaceRole?> UpdateSpaceRoleAsync(ISpaceRole spaceRole, Guid memberId);
    
    Task<bool> DeleteSpaceRoleAsync(Guid spaceId, Guid roleId, Guid memberId);
    
    Task<ISpaceMember> AddMemberToSpaceAsync(Guid spaceId, Guid userId, Guid roleId);
    
    Task<ISpaceMember?> UpdateSpaceMemberAsync(ISpaceMember spaceMember);
    
    Task<bool> RemoveMemberFromSpaceAsync(Guid spaceId, Guid userId);
}
