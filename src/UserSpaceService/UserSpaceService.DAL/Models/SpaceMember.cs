using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.DAL.Models;

public sealed class SpaceMember : ISpaceMember
{
    [Key]
    [Column("space_member_id")]
    public Guid Id { get; set; }
    
    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("space_username")]
    [MaxLength(255)]
    public string? SpaceUsername { get; set; } = null;
    
    [Column("role_id")]
    public Guid RoleId { get; set; }

    [ForeignKey("SpaceId")]
    public Space Space { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("RoleId")]
    public SpaceRole Role { get; set; } = null!;

    ISpaceRole ISpaceMember.Role
    {
        get => Role;
        set => Role = (SpaceRole)value;
    }

    ISpace ISpaceMember.Space
    {
        get => Space;
        set => Space = (Space)value;
    }
    
    IUser ISpaceMember.User
    {
        get => User;
        set => User = (User)value;
    }
}
