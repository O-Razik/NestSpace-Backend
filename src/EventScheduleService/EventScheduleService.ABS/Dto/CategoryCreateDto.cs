namespace EventScheduleService.ABS.Dto;

public class CreateCategoryDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}
