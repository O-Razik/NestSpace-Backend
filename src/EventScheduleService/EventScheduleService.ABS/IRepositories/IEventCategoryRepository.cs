using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IRepositories;

public interface IEventCategoryRepository
{
    Task<IEnumerable<EventCategory>> GetBySpaceAsync(Guid spaceId);
    
    Task<EventCategory?> GetByIdAsync(Guid eventId);
    
    Task<EventCategory> AddAsync(Guid spaceId, string title, string description);
    
    Task<EventCategory?> UpdateAsync(EventCategory updatedEvent);
    
    Task<bool> DeleteAsync(Guid eventId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}