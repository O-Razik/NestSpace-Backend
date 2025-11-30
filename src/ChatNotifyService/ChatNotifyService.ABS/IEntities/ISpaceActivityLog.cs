namespace ChatNotifyService.ABS.IEntities;

public interface ISpaceActivityLog
{
    Guid Id { get; set; }
    
    Guid SpaceId { get; set; }
    
    Guid MemberId { get; set; }
    
    string Type { get; set; }
    
    string Description { get; set; }
    
    DateTime Timestamp { get; set; }
}