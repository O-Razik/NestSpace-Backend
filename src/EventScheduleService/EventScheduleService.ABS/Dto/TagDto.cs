namespace EventScheduleService.ABS.Dto;

public class TagDto
{
    public Guid Id { get; init; }
    public Guid SpaceId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}
