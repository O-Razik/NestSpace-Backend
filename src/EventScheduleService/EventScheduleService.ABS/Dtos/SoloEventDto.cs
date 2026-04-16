namespace EventScheduleService.ABS.Dtos;

public class SoloEventDto
{
    public Guid Id { get; init; }
    public Guid SpaceId { get; init; }
    public Guid CategoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsYearly { get; init; }
    public IList<TagDto> Tags { get; init; } = [];
}
