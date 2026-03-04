using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IServices;

public interface ITokenService
{
    string GenerateAccessToken(IUser user);
    
    string GenerateRefreshToken();
    
    Task<IRefreshToken> SaveRefreshTokenAsync(Guid userId, string token);
    
    Task<IRefreshToken?> GetRefreshTokenAsync(string token);
    
    Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null);
    
    Task RevokeAllUserTokensAsync(Guid userId);
}