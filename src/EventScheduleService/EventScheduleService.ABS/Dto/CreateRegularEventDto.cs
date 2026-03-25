using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.Dto;

public class CreateRegularEventDto
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public Guid CategoryId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;
    
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    
    public Day Day { get; set; } = Day.Monday;
    
    public Frequency Frequency { get; set; } = Frequency.Weekly;
    
    public IList<TagDto> Tags { get; set; } = [];
}
