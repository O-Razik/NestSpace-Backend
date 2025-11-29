using EventScheduleService.ABS.IModels;

namespace EventScheduleService.BLL.Dto.Create;

public class RegularEventCreateDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public Guid CategoryId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeSpan Duration { get; set; }
    
    public Day Day { get; set; }
    
    public Frequency Frequency { get; set; }
    
    public List<Guid> TagIds { get; set; } = [];
}