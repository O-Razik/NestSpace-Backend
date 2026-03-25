using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IServices;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    
    string GenerateRefreshToken();
    
    Task<RefreshToken> SaveRefreshTokenAsync(Guid userId, string token);
    
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    
    Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null);
    
    Task RevokeAllUserTokensAsync(Guid userId);
}
