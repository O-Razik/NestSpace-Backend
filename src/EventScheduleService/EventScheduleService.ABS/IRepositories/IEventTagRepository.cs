using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IRepositories;

public interface IEventTagRepository
{
    Task<IEnumerable<IEventTag>> GetBySpaceAsync(Guid spaceId);
    
    Task<IEventTag?> GetByIdAsync(Guid tagId);
    
    Task<IEventTag> AddAsync(Guid spaceId, string title, string color);
    
    Task<IEventTag?> UpdateAsync(IEventTag updatedTag);
    
    Task<bool> DeleteAsync(Guid tagId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}
