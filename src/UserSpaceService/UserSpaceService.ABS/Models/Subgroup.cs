using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public class Subgroup
{
    [Key]
    [Column("subgroup_id")]
    public Guid Id { get; set; }

    [Column("space_id")]
    public Guid SpaceId { get; set; }

    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [ForeignKey("SpaceId")]
    public Space Space { get; set; } = null!;

    public ICollection<SpaceMember> Members { get; set; } = new List<SpaceMember>();
}