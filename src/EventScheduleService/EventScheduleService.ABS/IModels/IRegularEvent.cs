namespace EventScheduleService.ABS.IModels;

public enum Day
{
    Monday = 0,
    Tuesday = 1,
    Wednesday = 2,
    Thursday = 3,
    Friday = 4,
    Saturday = 5,
    Sunday = 6
}

public enum Frequency
{
    Weekly = 0,
    BiWeekly = 1,
    TriWeekly = 2,
    Monthly = 3
}

public interface IRegularEvent
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public Day Day { get; set; }
    
    public Frequency Frequency { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string Tittle { get; set; }
    
    public string Description { get; set; }
    
    public TimeOnly StartTime { get; set; }
    
    public TimeSpan Duration { get; set; }
    
    public IEventCategory Category { get; set; }
    
    public ICollection<IEventTag> Tags { get; set; }
}