using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.RabbitMQ.Events;

namespace EventScheduleService.BLL.Services;

public class EventService(
    IEventCategoryRepository categoryRepository,
    IEventTagRepository tagRepository,
    IEventPublisher eventPublisher
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
            var result = await categoryRepository.AddAsync(newEvent);

            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = newEvent.SpaceId,
                Type = "EventCategoryCreated",
                Description = $"Event category '{newEvent.Title}' created.",
                ActivityAt = DateTime.UtcNow
            };

            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );
            
            return result;
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
            var result = await categoryRepository.UpdateAsync(updatedEvent);
            
            if (result == null)
            {
                return null;
            }
            
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = updatedEvent.SpaceId,
                Type = "EventCategoryUpdated",
                Description = $"Event category '{updatedEvent.Title}' updated.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );
            
            return result;
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
            var result = await categoryRepository.DeleteAsync(eventId);

            if (!result) return result;
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = Guid.Empty, // Ideally, fetch the SpaceId associated with the eventId before deletion
                Type = "EventCategoryDeleted",
                Description = $"Event category with ID '{eventId}' deleted.",
                ActivityAt = DateTime.UtcNow
            };
                
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while deleting the event category.", e);
        }
    }

    public async Task<bool> DeleteCategoryBySpaceIdAsync(Guid spaceId)
    {
        try
        {
            return await categoryRepository.DeleteBySpaceIdAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
            var result = await tagRepository.AddAsync(newTag);

            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = newTag.SpaceId,
                Type = "EventTagCreated",
                Description = $"Event tag '{newTag.Title}' created.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );
            
            return result;
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
            var result = await tagRepository.UpdateAsync(updatedTag);
            
            if (result == null)
            {
                return null;
            }
            
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = updatedTag.SpaceId,
                Type = "EventTagUpdated",
                Description = $"Event tag '{updatedTag.Title}' updated.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );
            
            return result;
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
            var result = await tagRepository.DeleteAsync(markerId);
            
            if (!result) return result;
            
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = Guid.Empty, // Ideally, fetch the SpaceId associated with the markerId before deletion
                Type = "EventTagDeleted",
                Description = $"Event tag with ID '{markerId}' deleted.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );
            
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while deleting the event tag.", e);
        }
    }

    public async Task<bool> DeleteTagBySpaceIdAsync(Guid spaceId)
    {
        try
        {
            return await tagRepository.DeleteBySpaceIdAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}