using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IRepositories;

public interface IEventTagRepository
{
    Task<IEnumerable<IEventTag>> GetBySpaceAsync(Guid spaceId);
    
    Task<IEventTag?> GetByIdAsync(Guid markerId);
    
    Task<IEventTag> AddAsync(IEventTag newTag);
    
    Task<IEventTag?> UpdateAsync(IEventTag updatedTag);
    
    Task<bool> DeleteAsync(Guid markerId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}