namespace UserSpaceService.ABS.IModels;

public interface ISpaceRole
{
    Guid Id { get; set; }
    
    Guid SpaceId { get; set; }
    
    string Name { get; set; }
    
    ISpace Space { get; set; }
    
    Permission RolePermissions { get; set; }
    
    ICollection<ISpaceMember> Members { get; set; }
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
