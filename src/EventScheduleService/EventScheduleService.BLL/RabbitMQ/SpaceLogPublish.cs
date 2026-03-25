using EventScheduleService.ABS.IHelpers;
using EventScheduleService.BLL.RabbitMQ.Events;

namespace EventScheduleService.BLL.RabbitMQ;

public class SpaceLogPublish(
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public Task PublishSpaceActivityLogAsync(
        Guid spaceId, Guid memberId,
        string type, string description)
    {
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = spaceId,
            MemberId = memberId,
            Type = type,
            Description = description,
            ActivityAt = dateTimeProvider.UtcNow.DateTime
        };
        
        return eventPublisher.PublishAsync(
            logEvent,
            routingKey: "space.activity.log",
            exchangeName: "log.exchange"
        );
    }
}