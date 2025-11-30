using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.DAL.Models;

public class RegularEvent : IRegularEvent
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
    
    public ICollection<EventTag> Tags { get; set; } = null!;

    IEventCategory IRegularEvent.Category
    {
        get => Category;
        set => Category = (EventCategory)value;
    }
    
    ICollection<IEventTag> IRegularEvent.Tags
    {
        get => Tags.Cast<IEventTag>().ToList();
        set => Tags = value.Cast<EventTag>().ToList();
    }
}