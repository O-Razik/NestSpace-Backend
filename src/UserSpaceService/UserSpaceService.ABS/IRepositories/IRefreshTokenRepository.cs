using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.IRepositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(Guid userId, string token, DateTime expiresAt);
    
    Task<RefreshToken?> GetByTokenAsync(string token);
    
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    
    Task RevokeAsync(string token, string? replacedByToken = null);
    
    Task RevokeAllUserTokensAsync(Guid userId);
}
