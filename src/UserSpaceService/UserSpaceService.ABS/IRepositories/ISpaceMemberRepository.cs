using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceMemberRepository
{
    Task<ISpaceMember?> GetByIdAsync(Guid spaceId, Guid userId);
    
    Task<ISpaceMember> CreateAsync(Guid spaceId, Guid userId, Guid roleId);
    
    Task<ISpaceMember?> UpdateAsync(ISpaceMember spaceMember);
    
    Task<bool> DeleteAsync(Guid spaceId, Guid userId);
}
