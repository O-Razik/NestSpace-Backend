namespace EventScheduleService.BLL.RabbitMQ.Events;

public class SpaceActivityLogEvent
{
    public Guid SpaceId { get; set; }
    
    public Guid MemberId { get; set; }

    public string Type { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime ActivityAt { get; set; }
}
