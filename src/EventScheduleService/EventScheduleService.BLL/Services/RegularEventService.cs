using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.RabbitMQ.Events;

namespace EventScheduleService.BLL.Services;

public class RegularEventService(
    IRegularEventRepository regularEventRepository,
    IEventPublisher eventPublisher
) : IRegularEventService
{
    public async Task<IEnumerable<IRegularEvent>> GetRegularEventsBySpaceAsync(Guid spaceId)
    {
        return await regularEventRepository.GetAllBySpaceAsync(spaceId);
    }

    public async Task<IRegularEvent?> GetRegularEventByIdAsync(Guid regularEventId)
    {
        return await regularEventRepository.GetByIdAsync(regularEventId);
    }

    public async Task<IRegularEvent> CreateRegularEventAsync(IRegularEvent newRegularEvent)
    {
        try
        {
            var result = await regularEventRepository.AddAsync(newRegularEvent);
            
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = newRegularEvent.SpaceId,
                Type = "RegularEventCreated",
                Description = $"Regular event '{newRegularEvent.Title}' created.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.activity"
            );
            
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IRegularEvent?> UpdateRegularEventAsync(IRegularEvent updatedRegularEvent)
    {
        try
        {
            var result = await regularEventRepository.UpdateAsync(updatedRegularEvent);

            if (result == null) return result;
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = updatedRegularEvent.SpaceId,
                Type = "RegularEventUpdated",
                Description = $"Regular event '{updatedRegularEvent.Title}' updated.",
                ActivityAt = DateTime.UtcNow
            };
                
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.activity"
            );

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteRegularEventAsync(Guid regularEventId)
    {
        try
        {
            var result = await regularEventRepository.DeleteAsync(regularEventId);

            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = Guid.Empty, // Ideally, fetch the SpaceId before deletion for accurate logging
                Type = "RegularEventDeleted",
                Description = $"Regular event with ID '{regularEventId}' deleted.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.activity"
            );
            
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteRegularEventsBySpaceIdAsync(Guid spaceId)
    {
        try
        {
            return await regularEventRepository.DeleteBySpaceIdAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}