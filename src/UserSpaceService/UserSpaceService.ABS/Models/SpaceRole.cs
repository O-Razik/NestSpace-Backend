using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public sealed class SpaceRole
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
}

[Flags]
public enum Permission : long
{
    None = 0,
    SendMessage = 1 << 0, // Chat
    ManageNotes = 1 << 1,
    ManageTasks = 1 << 2,
    ManageEvents = 1 << 3,
    ManageSoloEvents = 1 << 4,
    ManageRegularEvents = 1 << 5,
    ManageTags = 1 << 6,
    ManageMembers = 1 << 7,
    ManageSpace = 1 << 8,
    DeleteSpace = 1 << 9,
    All = ~0L
}
