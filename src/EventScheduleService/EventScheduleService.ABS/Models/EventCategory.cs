using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventScheduleService.ABS.Models;

public class EventCategory
{
    [Key]
    [Column("event_id")]
    public Guid Id { get; set; }
    
    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("tittle")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Column("description")]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;
    
    public ICollection<SoloEvent> SoloEvents { get; set; } = new List<SoloEvent>();
    
    public ICollection<RegularEvent> RegularEvents { get; set; } = new List<RegularEvent>();
}
