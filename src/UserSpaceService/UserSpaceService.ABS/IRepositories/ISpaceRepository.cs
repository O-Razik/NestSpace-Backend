using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IRepositories;

public interface ISpaceRepository
{
    Task<IEnumerable<ISpace>> GetAllAsync();
    
    Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId);
    
    Task<ISpace?> GetByIdAsync(Guid spaceId);
    
    Task<ISpace> CreateAsync(Guid creatorId, string name);
    
    Task<ISpace?> UpdateAsync(ISpace updatedSpace);
    
    Task<bool> DeleteAsync(ISpace space);
}