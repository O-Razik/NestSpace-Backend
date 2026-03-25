using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IRepositories;

public interface IEventTagRepository
{
    Task<IEnumerable<EventTag>> GetBySpaceAsync(Guid spaceId);
    
    Task<EventTag?> GetByIdAsync(Guid tagId);
    
    Task<EventTag> AddAsync(Guid spaceId, string title, string color);
    
    Task<EventTag?> UpdateAsync(EventTag updatedTag);
    
    Task<bool> DeleteAsync(Guid tagId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}
