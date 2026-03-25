using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventScheduleService.ABS.Models;

public class RegularEvent
{
    [Key]
    [Column("regular_event_id")]
    public Guid Id { get; set; }

    [Column("space_id")]
    public Guid SpaceId { get; set; }

    [Column("day")]
    public Day Day { get; set; }
    
    [Column("frequency")]
    public Frequency Frequency { get; set; }

    [Column("event_id")]
    public Guid CategoryId { get; set; }
    
    [Column("title")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Column("description")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column("start_time", TypeName = "time")]
    public TimeOnly StartTime { get; set; }

    [Column("duration", TypeName = "time")]
    [Range(typeof(TimeSpan), "00:00:00", "24:00:00", ErrorMessage = "Duration must be between 0 and 24 hours.")]
    public TimeSpan Duration { get; set; }
    
    [ForeignKey("CategoryId")]
    public EventCategory Category { get; set; } = null!;
    
    public ICollection<EventTag> Tags { get; set; } = new List<EventTag>();
}

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
