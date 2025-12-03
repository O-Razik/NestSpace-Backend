using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.RabbitMQ.Events;

namespace EventScheduleService.BLL.Services;

public class SoloEventService(
    ISoloEventRepository soloEventRepository,
    IEventPublisher eventPublisher
) : ISoloEventService
{
    public async Task<IEnumerable<ISoloEvent>> GetSoloEventsBySpaceAsync(Guid spaceId)
    {
        return await soloEventRepository.GetBySpaceAsync(spaceId);
    }

    public async Task<ISoloEvent?> GetSoloEventByIdAsync(Guid soloEventId)
    {
        return await soloEventRepository.GetByIdAsync(soloEventId);
    }

    public async Task<ISoloEvent> CreateSoloEventAsync(ISoloEvent newSoloEvent)
    {
        try
        {
            var result = await soloEventRepository.AddAsync(newSoloEvent);
            
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = newSoloEvent.SpaceId,
                Type = "SoloEventCreated",
                Description = $"Solo event '{newSoloEvent.Title}' created.",
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
            throw;
        }
    }

    public async Task<ISoloEvent?> UpdateSoloEventAsync(ISoloEvent updatedSoloEvent)
    {
        try
        {
            var result = await soloEventRepository.UpdateAsync(updatedSoloEvent);
            
            if (result == null) return result;
            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = updatedSoloEvent.SpaceId,
                Type = "SoloEventUpdated",
                Description = $"Solo event '{updatedSoloEvent.Title}' updated.",
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
            throw;
        }
    }

    public async Task<bool> DeleteSoloEventAsync(Guid soloEventId)
    {
        try
        {
            var result = await soloEventRepository.DeleteAsync(soloEventId);

            var logEvent = new SpaceActivityLogEvent()
            {
                SpaceId = Guid.Empty, // You might want to fetch the actual SpaceId before deletion
                MemberId = Guid.Empty,
                Type = "SoloEventDeleted",
                Description = $"Solo event with ID '{soloEventId}' deleted.",
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
            throw;
        }
    }

    public async Task<bool> DeleteSoloEventsBySpaceIdAsync(Guid spaceId)
    {
        try
        {
            return await soloEventRepository.DeleteBySpaceIdAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}