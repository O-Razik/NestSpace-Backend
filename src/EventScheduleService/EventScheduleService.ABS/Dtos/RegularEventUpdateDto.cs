using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.Dtos;

public class UpdateRegularEventDto
{
    public Guid Id { get; init; }
    
    public Guid SpaceId { get; init; }
    
    public Guid CategoryId { get; init; }
    
    public Day Day { get; set; }
    
    public Frequency Frequency { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeSpan Duration { get; set; }
    
    public IList<Guid> TagIds { get; private set; } = [];
}
