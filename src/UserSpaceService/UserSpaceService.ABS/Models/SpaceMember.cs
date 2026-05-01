using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public sealed class SpaceMember
{
    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("space_username")]
    [MaxLength(255)]
    public string? SpaceUsername { get; set; }
    
    [Column("role_id")]
    public Guid RoleId { get; set; }
    
    [Column("joined_at")]
    public DateTimeOffset JoinedAt { get; set; }
    
    [Column("subgroup_id")]
    public Guid? SubgroupId { get; set; }

    [ForeignKey("SpaceId")]
    public Space Space { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("RoleId")]
    public SpaceRole Role { get; set; } = null!;
    
    [ForeignKey("SubgroupId")]
    public Subgroup? Subgroup { get; set; }
}
