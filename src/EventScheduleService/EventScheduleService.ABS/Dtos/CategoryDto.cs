namespace EventScheduleService.ABS.Dtos;

public class CategoryShortDto
{
    public Guid Id { get; init; }
    public Guid SpaceId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class CategoryDto : CategoryShortDto
{
    public IList<RegularEventDto> RegularEvents { get; init; } = [];
    public IList<SoloEventDto> SoloEvents { get; init; } = [];
}
