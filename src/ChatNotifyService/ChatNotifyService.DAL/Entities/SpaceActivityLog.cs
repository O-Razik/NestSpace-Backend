using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.DAL.Entities;

class SpaceActivityLog : ISpaceActivityLog
{
    [Key]
    [Column("log_id")]
    public Guid Id { get; set; }

    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("member_id")]
    public Guid MemberId { get; set; }

    [Column("type")]
    public string Type { get; set; } = null!;
    
    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}