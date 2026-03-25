namespace EventScheduleService.BLL.RabbitMQ.Events;

public class DeleteSpaceEvent
{
    public Guid SpaceId { get; set; }
    
    public DateTime DeletedAt { get; set; }
}
