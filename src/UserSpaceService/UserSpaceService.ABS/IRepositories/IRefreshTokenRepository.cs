using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.IRepositories;

public interface IRefreshTokenRepository
{
    Task<IRefreshToken> CreateAsync(Guid userId, string token, DateTime expiresAt);
    
    Task<IRefreshToken?> GetByTokenAsync(string token);
    
    Task<IEnumerable<IRefreshToken>> GetActiveByUserIdAsync(Guid userId);
    
    Task RevokeAsync(string token, string? replacedByToken = null);
    
    Task RevokeAllUserTokensAsync(Guid userId);
}