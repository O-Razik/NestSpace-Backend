using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceMemberRepository
{
    Task<SpaceMember?> GetByIdAsync(Guid spaceId, Guid userId);
    
    Task<SpaceMember> CreateAsync(Guid spaceId, Guid userId, Guid roleId);
    
    Task<SpaceMember?> UpdateAsync(SpaceMember spaceMember);
    
    Task<bool> DeleteAsync(Guid spaceId, Guid userId);
}
