namespace ChatNotifyService.BLL.Dtos.Create;

public class SpaceActivityLogCreateDto
{
    public Guid MemberId { get; set; }
    
    public string Type { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}