using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IServices;

public interface ISpaceService
{
    Task<PagedResult<Space>> SearchSpacesAsync(SpaceFilter filter);
    
    Task<IEnumerable<Space>> GetAllSpacesOfUserAsync(Guid userId);
    
    Task<Space?> GetSpaceByIdAsync(Guid spaceId);
    
    Task<Space> CreateSpaceAsync(CreateSpaceDto createSpaceDto);
    
    Task<Space?> UpdateSpaceNameAsync(Guid spaceId, string newName, Guid memberId);
    
    Task<bool> DeleteSpaceAsync(Guid spaceId);
    
    Task<SpaceRole> CreateSpaceRoleAsync(Guid spaceId, string roleName, Permission permissions, Guid memberId);
    
    Task<SpaceRole?> UpdateSpaceRoleAsync(SpaceRole spaceRole, Guid memberId);
    
    Task<bool> DeleteSpaceRoleAsync(Guid spaceId, Guid roleId, Guid memberId);
    
    Task<SpaceMember> AddMemberToSpaceAsync(Guid spaceId, Guid userId, Guid roleId);
    
    Task<SpaceMember?> UpdateSpaceMemberAsync(SpaceMember spaceMember);
    
    Task<bool> RemoveMemberFromSpaceAsync(Guid spaceId, Guid userId);
}
