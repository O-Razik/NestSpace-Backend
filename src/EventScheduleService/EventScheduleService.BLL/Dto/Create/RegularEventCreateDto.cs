using EventScheduleService.ABS.IModels;

namespace EventScheduleService.BLL.Dto.Create;

public class RegularEventCreateDto
{
    public Guid CategoryId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;
    
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    
    public Day Day { get; set; } = Day.Monday;
    
    public Frequency Frequency { get; set; } = Frequency.Weekly;
    
    public List<Guid> TagIds { get; set; } = [];
}