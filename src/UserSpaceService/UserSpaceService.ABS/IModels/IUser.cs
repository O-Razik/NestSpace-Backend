namespace UserSpaceService.ABS.IModels;

public interface IUser
{
    Guid Id { get; set; }
    
    string Username { get; set; }
    
    string NormalizedUsername { get; set; }
    
    string Email { get; set; }
    
    string NormalizedEmail { get; set; }
    
    string PasswordHash { get; set; }
    
    string SecurityStamp { get; set; }
    
    ICollection<IExternalLogin> ExternalLogins { get; set; }
    
    ICollection<ISpaceMember> SpaceMemberships { get; set; }
}