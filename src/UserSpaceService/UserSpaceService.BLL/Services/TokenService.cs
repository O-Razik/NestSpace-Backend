using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.BLL.Services;

public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration,
    IDateTimeProvider dateTimeProvider
        ) : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        Guard.AgainstNull(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = configuration["Jwt:Key"];
        Guard.AgainstNullOrWhiteSpace(key);
        
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            Expires = dateTimeProvider.UtcNow.DateTime.AddMinutes(15),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<RefreshToken> SaveRefreshTokenAsync(Guid userId, string token)
    {
        var expiresAt = dateTimeProvider.UtcNow.DateTime.AddDays(7); // 7 днів
        return await refreshTokenRepository.CreateAsync(userId, token, expiresAt);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await refreshTokenRepository.GetByTokenAsync(token);
    }

    public async Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null)
    {
        await refreshTokenRepository.RevokeAsync(token, replacedByToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        await refreshTokenRepository.RevokeAllUserTokensAsync(userId);
    }
}