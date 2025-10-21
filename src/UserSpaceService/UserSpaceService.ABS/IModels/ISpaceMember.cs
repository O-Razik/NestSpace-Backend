namespace UserSpaceService.ABS.IModels;

public interface ISpaceMember
{
    Guid Id { get; set; }
    
    Guid SpaceId { get; set; }
    
    Guid UserId { get; set; }
    
    string? SpaceUsername { get; set; }
    
    Guid RoleId { get; set; }
    
    ISpace Space { get; set; }
    
    IUser User { get; set; }
    
    ISpaceRole Role { get; set; }
}