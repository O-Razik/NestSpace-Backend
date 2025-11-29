namespace EventScheduleService.BLL.Dto.Create;

public class SoloEventCreateDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public Guid CategoryId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsYearly { get; set; } = false;
    
    public List<Guid> TagIds { get; set; } = [];
}