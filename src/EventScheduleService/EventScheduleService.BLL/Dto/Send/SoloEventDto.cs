namespace EventScheduleService.BLL.Dto.Send;

public class SoloEventDto
{
    public Guid Id { get; set; }
    public Guid SpaceId { get; set; }
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsYearly { get; set; }
    public List<TagDto> Tags { get; set; } = [];
}