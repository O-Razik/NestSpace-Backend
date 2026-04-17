using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceRepository
{
    Task<PagedResult<Space>> SearchAsync(SpaceFilter filter);
    
    Task<Space?> SearchByNameAsync(string name);
    
    Task<IEnumerable<Space>> GetAllSpacesOfUserAsync(Guid userId);
    
    Task<Space?> GetByIdAsync(Guid spaceId);
    
    Task<Space> CreateAsync(Guid creatorId, string name, IList<Guid> memberIds);
    
    Task<Space?> UpdateAsync(Space updatedSpace);
    
    Task<bool> DeleteAsync(Guid spaceId);
}
