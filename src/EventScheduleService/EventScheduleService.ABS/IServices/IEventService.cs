using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IServices;

public interface IEventService
{
    Task<IEnumerable<IEventCategory>> GetCategoriesBySpaceAsync(Guid spaceId);
    
    Task<IEventCategory?> GetCategoryByIdAsync(Guid eventId);
    
    Task<IEventCategory> CreateCategoryAsync(IEventCategory newEvent);
    
    Task<IEventCategory?> UpdateCategoryAsync(IEventCategory updatedEvent);
    
    Task<bool> DeleteCategoryAsync(Guid eventId);
    
    Task<IEnumerable<IEventTag>> GetMarkersBySpaceAsync(Guid spaceId);
    
    Task<IEventTag?> GetMarkerByIdAsync(Guid markerId);
    
    Task<IEventTag> CreateMarkerAsync(IEventTag newTag);
    
    Task<IEventTag?> UpdateMarkerAsync(IEventTag updatedTag);
    
    Task<bool> DeleteMarkerAsync(Guid markerId);
}