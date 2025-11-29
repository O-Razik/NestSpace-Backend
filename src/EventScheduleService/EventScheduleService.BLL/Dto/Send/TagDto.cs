namespace EventScheduleService.BLL.Dto.Send;

public class TagDto
{
    public Guid Id { get; set; }
    public Guid SpaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}