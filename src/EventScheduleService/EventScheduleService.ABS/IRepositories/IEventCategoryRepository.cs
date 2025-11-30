using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IRepositories;

public interface IEventCategoryRepository
{
    Task<IEnumerable<IEventCategory>> GetBySpaceAsync(Guid spaceId);
    
    Task<IEventCategory?> GetByIdAsync(Guid eventId);
    
    Task<IEventCategory> AddAsync(IEventCategory newEvent);
    
    Task<IEventCategory?> UpdateAsync(IEventCategory updatedEvent);
    
    Task<bool> DeleteAsync(Guid eventId);
}