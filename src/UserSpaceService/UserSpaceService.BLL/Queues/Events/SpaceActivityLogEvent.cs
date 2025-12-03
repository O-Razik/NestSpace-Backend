namespace UserSpaceService.BLL.Queues.Events;

public class SpaceActivityLogEvent
{
    public Guid SpaceId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public string Type { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public DateTime ActivityAt { get; set; }
}