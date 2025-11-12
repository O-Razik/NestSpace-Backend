namespace ChatNotifyService.BLL.RabbitMQ;

public class ChatCreateEvent
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    public Guid ChatId { get; set; } = Guid.Empty;
    
    public Guid MemberId { get; set; } = Guid.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
}