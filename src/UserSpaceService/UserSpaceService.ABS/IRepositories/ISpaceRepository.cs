using System.Collections.ObjectModel;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceRepository
{
    Task<PagedResult<ISpace>> SearchAsync(SpaceFilter filter);
    
    Task<ISpace?> SearchByNameAsync(string name);
    
    Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId);
    
    Task<ISpace?> GetByIdAsync(Guid spaceId);
    
    Task<ISpace> CreateAsync(Guid creatorId, string name, IList<Guid> memberIds);
    
    Task<ISpace?> UpdateAsync(ISpace updatedSpace);
    
    Task<bool> DeleteAsync(Guid spaceId);
}
