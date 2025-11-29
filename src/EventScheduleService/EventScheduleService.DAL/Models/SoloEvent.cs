using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.DAL.Models;

public class SoloEvent : ISoloEvent
{
    [Key] [Column("solo_event_id")]
    public Guid Id { get; set; }

    [Column("space_id")] 
    public Guid SpaceId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("title")] [MaxLength(255)]
    public required string Title { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("is_yearly")]
    public bool IsYearly { get; set; }

    [ForeignKey("CategoryId")]
    public EventCategory Category { get; set; } = null!;

    public ICollection<EventTag> Tags { get; set; } = null!;

    IEventCategory ISoloEvent.Category
    {
        get => Category;
        set => Category = (value as EventCategory)!;
    }

    ICollection<IEventTag> ISoloEvent.Tags
    {
        get => Tags.Cast<IEventTag>().ToList();
        set => Tags = value.Cast<EventTag>().ToList();
    }
}