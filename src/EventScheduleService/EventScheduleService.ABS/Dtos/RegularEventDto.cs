using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.Dtos;

public class RegularEventDto
{
    public Guid Id { get; init; }
    public Guid SpaceId { get; init; }
    public Guid CategoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TimeOnly StartTime { get; init; }
    public TimeSpan Duration { get; init; }
    public Day Day { get; init; }
    public Frequency Frequency { get; init; }
    public IList<TagDto> Tags { get; init; } = [];
}
