using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Repositories;

public class RefreshTokenRepository(
    UserSpaceDbContext context,
    IDateTimeProvider dateTimeProvider
    ) : IRefreshTokenRepository
{
    public async Task<IRefreshToken> CreateAsync(Guid userId, string token, DateTime expiresAt)
    {
        var refreshToken = new RefreshToken(dateTimeProvider)
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = dateTimeProvider.UtcNow
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<IRefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<IRefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > dateTimeProvider.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeAsync(string token, string? replacedByToken = null)
    {
        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken != null)
        {
            refreshToken.RevokedAt = dateTimeProvider.UtcNow;
            refreshToken.ReplacedByToken = replacedByToken;
            await context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}
