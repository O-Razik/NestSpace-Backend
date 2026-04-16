namespace ChatNotifyService.ABS.Dtos;

public class SpaceActivityLogCreateDto
{
    public Guid SpaceId { get; set; }
    
    public Guid MemberId { get; set; }
    
    public string Type { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}
