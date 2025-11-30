namespace EventScheduleService.BLL.Dto.Send;

public class CategoryShortDto
{
    public Guid Id { get; set; }
    public Guid SpaceId { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class CategoryDto : CategoryShortDto
{
    public string Description { get; set; } = string.Empty;
    public List<RegularEventDto> RegularEvents { get; set; } = [];
    public List<SoloEventDto> SoloEvents { get; set; } = [];
}