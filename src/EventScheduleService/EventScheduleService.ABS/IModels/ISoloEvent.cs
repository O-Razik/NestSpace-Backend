namespace EventScheduleService.ABS.IModels;

public interface ISoloEvent
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsYearly { get; set; }
    
    IEventCategory EventCategory { get; set; }
    
    ICollection<IEventTag> Tags { get; set; }
}