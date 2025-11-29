using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.DAL.Models;

public class EventTag : IEventTag
{
    [Key]
    [Column("marker_id")]
    public Guid Id { get; set; }
    
    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("title")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Column("color")]
    [MaxLength(255)]
    public string Color { get; set; } = string.Empty;
    
    public ICollection<SoloEvent> SoloEvents { get; set; } = null!;

    public ICollection<RegularEvent> RegularEvents { get; set; } = null!;

    ICollection<ISoloEvent> IEventTag.SoloEvents
    {
        get => SoloEvents.Cast<ISoloEvent>().ToList();
        set => SoloEvents = value.Cast<SoloEvent>().ToList();
    }
    
    ICollection<IRegularEvent> IEventTag.RegularEvents
    {
        get => RegularEvents.Cast<IRegularEvent>().ToList();
        set => RegularEvents = value.Cast<RegularEvent>().ToList();
    }
}