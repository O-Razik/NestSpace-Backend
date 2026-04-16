namespace ChatNotifyService.ABS.Dtos;

public class SpaceActivityLogDto
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public string Type { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
}
