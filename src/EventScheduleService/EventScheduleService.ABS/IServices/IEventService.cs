using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IServices;

public interface IEventService
{
    Task<IEnumerable<EventCategory>> GetCategoriesBySpaceAsync(Guid spaceId);
    
    Task<EventCategory?> GetCategoryByIdAsync(Guid eventId);
    
    Task<EventCategory> CreateCategoryAsync(CreateCategoryDto newCategory);
    
    Task<EventCategory?> UpdateCategoryAsync(EventCategory updatedCategory);
    
    Task<bool> DeleteCategoryAsync(Guid eventId);
    
    Task<bool> DeleteCategoryBySpaceIdAsync(Guid spaceId);
    
    Task<IEnumerable<EventTag>> GetTagsBySpaceAsync(Guid spaceId);
    
    Task<EventTag?> GetTagByIdAsync(Guid markerId);
    
    Task<EventTag> CreateTagAsync(CreateTagDto newTag);
    
    Task<EventTag?> UpdateTagAsync(EventTag updatedTag);
    
    Task<bool> DeleteTagAsync(Guid tagId);
    
    Task<bool> DeleteTagBySpaceIdAsync(Guid spaceId);
}
