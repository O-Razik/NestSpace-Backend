namespace EventScheduleService.ABS.Dtos;

public class CategoryCreateDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}
