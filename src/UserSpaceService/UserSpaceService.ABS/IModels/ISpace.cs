namespace UserSpaceService.ABS.IModels;

public interface ISpace
{
    Guid Id { get; set; }
    
    string Name { get; set; }
    
    ICollection<ISpaceMember> Members { get; set; }
    
    ICollection<ISpaceRole> Roles { get; set; }
}