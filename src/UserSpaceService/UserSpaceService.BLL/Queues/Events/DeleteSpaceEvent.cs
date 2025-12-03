namespace UserSpaceService.BLL.Queues.Events;

public class DeleteSpaceEvent
{
    public Guid SpaceId { get; set; }
    
    public DateTime DeletedAt { get; set; }
}