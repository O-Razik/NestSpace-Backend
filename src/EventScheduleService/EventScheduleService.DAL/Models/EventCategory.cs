using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventScheduleService.ABS.IModels;

namespace EventScheduleService.DAL.Models;

public class EventCategory : IEventCategory
{
    [Key]
    [Column("event_id")]
    public Guid Id { get; set; }
    
    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("tittle")]
    [MaxLength(255)]
    public required string Tittle { get; set; }
    
    [Column("description")]
    [MaxLength(255)]
    public required string Description { get; set; }
    
    public ICollection<SoloEvent> SoloEvents { get; set; } = new List<SoloEvent>();
    
    public ICollection<RegularEvent> RegularEvents { get; set; } = new List<RegularEvent>();
    
    ICollection<ISoloEvent> IEventCategory.SoloEvents
    {
        get => SoloEvents.Cast<ISoloEvent>().ToList();
        set => SoloEvents = value.Cast<SoloEvent>().ToList();
    }
    
    ICollection<IRegularEvent> IEventCategory.RegularEvents
    {
        get => RegularEvents.Cast<IRegularEvent>().ToList();
        set => RegularEvents = value.Cast<RegularEvent>().ToList();
    }
}