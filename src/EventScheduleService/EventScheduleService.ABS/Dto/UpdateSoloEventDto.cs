namespace EventScheduleService.ABS.Dto;

public class UpdateSoloEventDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsYearly { get; set; }
    
    public IList<Guid> TagIds { get; private set; } = [];
}
