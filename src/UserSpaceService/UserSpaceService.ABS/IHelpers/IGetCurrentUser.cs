namespace UserSpaceService.ABS.IHelpers;

public interface IGetCurrentUser
{
    Guid UserId();
    
    string? Email { get; }
    
    string? Username { get; }
    
    bool IsAuthenticated { get; }
}
