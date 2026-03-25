using EventScheduleService.ABS.IHelpers;

namespace EventScheduleService.ABS.Dto;

public class CreateSoloEventDto
    (IDateTimeProvider dateTimeProvider)
{
    public Guid SpaceId { get; set; } = Guid.Empty;
    
    public Guid CategoryId { get; set; } = Guid.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = dateTimeProvider.UtcNow.DateTime;
    
    public DateTime EndDate { get; set; } = dateTimeProvider.UtcNow.DateTime;

    public bool IsYearly { get; set; }

    public IList<TagDto> Tags { get; private set; } = [];
}
