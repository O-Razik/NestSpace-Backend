using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.DAL.Models;

public sealed class SpaceRole : ISpaceRole
{
    [Key]
    [Column("role_id")]
    public Guid Id { get; set; }
    
    [Column("space_id")]
    public Guid SpaceId { get; set; }
    
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Column("role_permissions")]
    public Permission RolePermissions { get; set; } = Permission.None;
    
    [ForeignKey("SpaceId")]
    public Space Space { get; set; } = null!;
    
    public ICollection<SpaceMember> Members { get; private set; } = new List<SpaceMember>();

    ISpace ISpaceRole.Space
    {
        get => Space;
        set => Space = (Space)value;
    }

    ICollection<ISpaceMember> ISpaceRole.Members
    {
        get => Members.Cast<ISpaceMember>().ToList();
        set => Members = value.Cast<SpaceMember>().ToList();
    }
}
