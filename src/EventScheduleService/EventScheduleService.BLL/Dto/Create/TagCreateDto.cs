namespace EventScheduleService.BLL.Dto.Create;

public class TagCreateDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Color { get; set; } = string.Empty;
}