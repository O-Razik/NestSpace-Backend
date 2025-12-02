namespace UserSpaceService.BLL.Queues;

public class DeleteSpaceEvent
{
    public Guid SpaceId { get; set; }
    
    public DateTime DeletedAt { get; set; }
}