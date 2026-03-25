using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventScheduleService.ABS.Models;

public class SoloEvent
{
    [Key]
    [Column("solo_event_id")]
    public Guid Id { get; set; }

    [Column("space_id")] 
    public Guid SpaceId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("title")] 
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

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

    public ICollection<EventTag> Tags { get; set; } = new List<EventTag>();
}
