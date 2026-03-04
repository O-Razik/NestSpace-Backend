namespace UserSpaceService.ABS.IModels;

public interface IRefreshToken
{
    Guid Id { get; set; }
    
    Guid UserId { get; set; }
    
    string Token { get; set; }
    
    DateTime ExpiresAt { get; set; }
    
    DateTimeOffset CreatedAt { get; set; }
    
    DateTimeOffset? RevokedAt { get; set; }
    
    string? ReplacedByToken { get; set; }
    
    IUser User { get; set; }
    
    bool IsActive { get; }
}
