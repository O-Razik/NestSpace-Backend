using EventScheduleService.ABS.IModels;

namespace EventScheduleService.BLL.Dto.Update;

public class RegularEventUpdateDto
{
    public Guid Id { get; set; }
    
    public Day Day { get; set; }
    
    public Frequency Frequency { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeSpan Duration { get; set; }
    
    public List<Guid> TagIds { get; set; } = [];
}