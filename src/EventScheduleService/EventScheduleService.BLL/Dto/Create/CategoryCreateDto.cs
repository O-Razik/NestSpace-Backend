namespace EventScheduleService.BLL.Dto.Create;

public class CategoryCreateDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public string Tittle { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
}