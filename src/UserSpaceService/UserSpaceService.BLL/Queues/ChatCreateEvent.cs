namespace UserSpaceService.BLL.Queues;

public class ChatCreateEvent
{
    public Guid SpaceId { get; set; }
    
    public Guid ChatId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public DateTime CreatedAt { get; set; }
}