using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.Helpers;
using EventScheduleService.BLL.RabbitMQ;

namespace EventScheduleService.BLL.Services;

public class EventService(
    IEventCategoryRepository categoryRepository,
    IEventTagRepository tagRepository,
    SpaceLogPublish logPublish
    ) : IEventService
{
    public async Task<IEnumerable<EventCategory>> GetCategoriesBySpaceAsync(Guid spaceId)
    {
        return await categoryRepository.GetBySpaceAsync(spaceId);
    }

    public async Task<EventCategory?> GetCategoryByIdAsync(Guid eventId)
    {
        return await categoryRepository.GetByIdAsync(eventId);
    }

    public async Task<EventCategory> CreateCategoryAsync(CreateCategoryDto newCategory)
    {
        Guard.AgainstNull(newCategory);

        var result = await categoryRepository
            .AddAsync(newCategory.SpaceId, newCategory.Title, newCategory.Description);

        await logPublish.PublishSpaceActivityLogAsync(
            newCategory.SpaceId, Guid.Empty, // MemberId can be added if available
            "EventCategoryCreated", 
            $"Event category '{newCategory.Title}' created.");
            
        return result;
    }

    public async Task<EventCategory?> UpdateCategoryAsync(EventCategory updatedCategory)
    {
        Guard.AgainstNull(updatedCategory);
        var result = await categoryRepository.UpdateAsync(updatedCategory);
            
        if (result != null)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                updatedCategory.SpaceId, Guid.Empty, // MemberId can be added if available
                "EventCategoryUpdated", 
                $"Event category '{updatedCategory.Title}' updated.");
        }
            
        return result;
    }

    public async Task<bool> DeleteCategoryAsync(Guid eventId)
    {
        var result = await categoryRepository.DeleteAsync(eventId);

        if (result)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                Guid.Empty, Guid.Empty, // SpaceId and MemberId can be added if available
                "EventCategoryDeleted",
                $"Event category with ID '{eventId}' deleted.");
        }

        return result;
    }

    public async Task<bool> DeleteCategoryBySpaceIdAsync(Guid spaceId)
    {
        return await categoryRepository.DeleteBySpaceIdAsync(spaceId);
    }

    public async Task<IEnumerable<EventTag>> GetTagsBySpaceAsync(Guid spaceId)
    {
        return await tagRepository.GetBySpaceAsync(spaceId);
    }

    public async Task<EventTag?> GetTagByIdAsync(Guid markerId)
    {
        return await tagRepository.GetByIdAsync(markerId);
    }

    public async Task<EventTag> CreateTagAsync(CreateTagDto newTag)
    {
        Guard.AgainstNull(newTag);
        var result = await tagRepository
            .AddAsync(newTag.SpaceId, newTag.Title, newTag.Color);
            
        await logPublish.PublishSpaceActivityLogAsync(
            newTag.SpaceId, Guid.Empty, // MemberId can be added if available
            "EventTagCreated", 
            $"Event tag '{newTag.Title}' created.");
            
        return result;
    }

    public async Task<EventTag?> UpdateTagAsync(EventTag updatedTag)
    {
        Guard.AgainstNull(updatedTag);
        
        var result = await tagRepository.UpdateAsync(updatedTag);
            
        if (result != null)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                updatedTag.SpaceId, Guid.Empty, // MemberId can be added if available
                "EventTagUpdated", 
                $"Event tag '{updatedTag.Title}' updated.");
        }
            
        return result;
    }

    public async Task<bool> DeleteTagAsync(Guid tagId)
    {
        var result = await tagRepository.DeleteAsync(tagId);
            
        if (result)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                Guid.Empty, Guid.Empty, // SpaceId and MemberId can be added if available
                "EventTagDeleted", 
                $"Event tag with ID '{tagId}' deleted.");
        }
        return result;
    }

    public async Task<bool> DeleteTagBySpaceIdAsync(Guid spaceId)
    {
        return await tagRepository.DeleteBySpaceIdAsync(spaceId);
    }
}
