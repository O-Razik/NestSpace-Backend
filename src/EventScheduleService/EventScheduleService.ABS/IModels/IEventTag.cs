namespace EventScheduleService.ABS.IModels;

public interface IEventTag
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public string Title { get; set; }
    
    public string Color { get; set; }
    
    public ICollection<ISoloEvent> SoloEvents { get; set; }
    
    public ICollection<IRegularEvent> RegularEvents { get; set; }
}