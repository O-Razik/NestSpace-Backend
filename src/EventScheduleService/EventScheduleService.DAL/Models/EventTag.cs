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
    public required string Title { get; set; }
    
    [Column("color")]
    [MaxLength(255)]
    public required string Color { get; set; }
    
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