using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;

namespace EventScheduleService.BLL.Services;

public class EventService(
    IEventCategoryRepository categoryRepository,
    IEventTagRepository tagRepository
    ) : IEventService
{
    public async Task<IEnumerable<IEventCategory>> GetCategoriesBySpaceAsync(Guid spaceId)
    {
        try
        {
            return await categoryRepository.GetBySpaceAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while retrieving event categories for the space.", e);
        }
    }

    public async Task<IEventCategory?> GetCategoryByIdAsync(Guid eventId)
    {
        try
        {
            return await categoryRepository.GetByIdAsync(eventId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while retrieving the event category.", e);
        }
    }

    public async Task<IEventCategory> CreateCategoryAsync(IEventCategory newEvent)
    {
        if (newEvent == null)
        {
            throw new ArgumentNullException(nameof(newEvent), "Event category cannot be null.");
        }

        try
        {
            return await categoryRepository.AddAsync(newEvent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while creating the event category.", e);
        }
    }

    public async Task<IEventCategory?> UpdateCategoryAsync(IEventCategory updatedEvent)
    {
        if (updatedEvent == null)
        {
            throw new ArgumentNullException(nameof(updatedEvent), "Event category cannot be null.");
        }

        try
        {
            return await categoryRepository.UpdateAsync(updatedEvent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while updating the event category.", e);
        }
    }

    public async Task<bool> DeleteCategoryAsync(Guid eventId)
    {
        try
        {
            return await categoryRepository.DeleteAsync(eventId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while deleting the event category.", e);
        }
    }

    public async Task<IEnumerable<IEventTag>> GetTagsBySpaceAsync(Guid spaceId)
    {
        try
        {
            return await tagRepository.GetBySpaceAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while retrieving event tags for the space.", e);
        }
    }

    public async Task<IEventTag?> GetTagByIdAsync(Guid markerId)
    {
        try
        {
            return await tagRepository.GetByIdAsync(markerId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while retrieving the event tag.", e);
        }
    }

    public async Task<IEventTag> CreateTagAsync(IEventTag newTag)
    {
        if (newTag == null)
        {
            throw new ArgumentNullException(nameof(newTag), "Event tag cannot be null.");
        }

        try
        {
            return await tagRepository.AddAsync(newTag);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while creating the event tag.", e);
        }
    }

    public async Task<IEventTag?> UpdateTagAsync(IEventTag updatedTag)
    {
        if (updatedTag == null)
        {
            throw new ArgumentNullException(nameof(updatedTag), "Event tag cannot be null.");
        }

        try
        {
            return await tagRepository.UpdateAsync(updatedTag);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while updating the event tag.", e);
        }
    }

    public async Task<bool> DeleteTagAsync(Guid markerId)
    {
        try
        {
            return await tagRepository.DeleteAsync(markerId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while deleting the event tag.", e);
        }
    }
}