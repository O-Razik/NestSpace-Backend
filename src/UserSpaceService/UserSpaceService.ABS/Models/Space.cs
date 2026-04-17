using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public sealed class Space
{
    [Key]
    [Column("space_id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<SpaceMember> Members { get; set; } = new List<SpaceMember>();
    
    public ICollection<SpaceRole> Roles { get; set; } = new List<SpaceRole>();
}
