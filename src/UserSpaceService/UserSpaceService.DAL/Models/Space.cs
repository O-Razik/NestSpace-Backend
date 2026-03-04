using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.DAL.Models;

public sealed class Space : ISpace
{
    [Key]
    [Column("space_id")]
    public Guid Id { get; set; }
    
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<SpaceMember> Members { get; private set; } = new List<SpaceMember>();
    
    public ICollection<SpaceRole> Roles { get; private set; } = new List<SpaceRole>();

    ICollection<ISpaceMember> ISpace.Members
    {
        get => Members.Cast<ISpaceMember>().ToList();
        set => Members = value.Cast<SpaceMember>().ToList();
    }
    
    ICollection<ISpaceRole> ISpace.Roles
    {
        get => Roles.Cast<ISpaceRole>().ToList();
        set => Roles = value.Cast<SpaceRole>().ToList();
    }
}
