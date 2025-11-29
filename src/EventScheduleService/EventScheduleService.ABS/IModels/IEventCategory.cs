namespace EventScheduleService.ABS.IModels;

public interface IEventCategory
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public ICollection<ISoloEvent> SoloEvents { get; set; }
    
    public ICollection<IRegularEvent> RegularEvents { get; set; }
}