using EventScheduleService.ABS.IModels;

namespace EventScheduleService.BLL.Dto.Send;

public class RegularEventDto
{
    public Guid Id { get; set; }
    public Guid SpaceId { get; set; }
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public Day Day { get; set; }
    public Frequency Frequency { get; set; }
    public List<TagDto> Tags { get; set; } = [];
    
}