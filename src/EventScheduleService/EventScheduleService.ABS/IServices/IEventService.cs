using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IServices;

public interface IEventService
{
    Task<IEnumerable<IEventCategory>> GetCategoriesBySpaceAsync(Guid spaceId);
    
    Task<IEventCategory?> GetCategoryByIdAsync(Guid eventId);
    
    Task<IEventCategory> CreateCategoryAsync(CreateCategoryDto newCategory);
    
    Task<IEventCategory?> UpdateCategoryAsync(IEventCategory updatedCategory);
    
    Task<bool> DeleteCategoryAsync(Guid eventId);
    
    Task<bool> DeleteCategoryBySpaceIdAsync(Guid spaceId);
    
    Task<IEnumerable<IEventTag>> GetTagsBySpaceAsync(Guid spaceId);
    
    Task<IEventTag?> GetTagByIdAsync(Guid markerId);
    
    Task<IEventTag> CreateTagAsync(CreateTagDto newTag);
    
    Task<IEventTag?> UpdateTagAsync(IEventTag updatedTag);
    
    Task<bool> DeleteTagAsync(Guid tagId);
    
    Task<bool> DeleteTagBySpaceIdAsync(Guid spaceId);
}