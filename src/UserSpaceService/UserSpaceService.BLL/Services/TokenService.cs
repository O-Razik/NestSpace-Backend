using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;

namespace UserSpaceService.BLL.Services;

public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(IUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key), "JWT key is not configured.");
        }
        
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15), // 15 хвилин!
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

    public async Task<IRefreshToken> SaveRefreshTokenAsync(Guid userId, string token)
    {
        var expiresAt = DateTime.UtcNow.AddDays(7); // 7 днів
        return await refreshTokenRepository.CreateAsync(userId, token, expiresAt);
    }

    public async Task<IRefreshToken?> GetRefreshTokenAsync(string token)
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