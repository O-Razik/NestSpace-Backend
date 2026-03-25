using Microsoft.Extensions.Configuration;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.BLL.Services;

public class UserService(
    IUserRepository repository,
    IConfiguration configuration,
    PasswordService passwordService,
    ITokenService tokenService)
    : IUserService
{

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var passwordHash = passwordService.HashPassword(registerDto.Password);
        var user = await repository.CreateAsync(registerDto.Username, registerDto.Email, passwordHash);
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RegisterByExternalProviderAsync(Provider provider, string providerUserId, string email)
    {
        Guard.AgainstNullOrEmpty(providerUserId, "Provider user ID cannot be null or empty.");
        Guard.AgainstNullOrEmpty(email, "Email cannot be null or empty.");
        
        var baseUsername = GetUsernameFromEmail(email);
        var uniqueUsername = await EnsureUniqueUsernameAsync(baseUsername);
        var user = await repository.CreateAsync(uniqueUsername, email, provider, providerUserId);
        
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await repository.GetByEmailAsync(loginDto.Email);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var valid = passwordService.VerifyPassword(loginDto.Password, user.PasswordHash);
        if (!valid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto?> LoginByExternalProviderAsync(Provider provider, string providerUserId)
    {
        Guard.AgainstNullOrEmpty(providerUserId, "Provider user ID cannot be null or empty.");
        var user = await repository.GetByExternalLoginAsync(provider, providerUserId);
        return user == null ?  null : await GenerateAuthResponseAsync(user);
    }
    
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        Guard.AgainstNullOrEmpty(refreshToken, "Refresh token cannot be null or empty.");
        var token = await tokenService.GetRefreshTokenAsync(refreshToken);

        if (token is not { IsActive: true })
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var newRefreshTokenValue = tokenService.GenerateRefreshToken();
        await tokenService.RevokeRefreshTokenAsync(refreshToken, newRefreshTokenValue);

        return await GenerateAuthResponseAsync(token.User);
    }
    
    public async Task LogoutAsync(string refreshToken)
    {
        Guard.AgainstNullOrEmpty(refreshToken, "Refresh token cannot be null or empty.");
        await tokenService.RevokeRefreshTokenAsync(refreshToken);
    }

    public async Task<User?> AddExternalLoginAsync(Guid userId, Provider provider, string providerUserId)
    {
        Guard.AgainstEmptyGuid(userId, "User ID cannot be empty.");
        Guard.AgainstNullOrEmpty(providerUserId, "Provider user ID cannot be null or empty.");

        var user = await repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (user.ExternalLogins.Any(el => el.Provider == provider && el.ProviderKey == providerUserId))
        {
            return user;
        }
        
        await repository.AddExternalLoginAsync(user, provider, providerUserId);
        
        return user;
    }

    public async Task<PagedResult<User>> SearchUsersAsync(UserFilter filter)
    {
        return await repository.SearchAsync(filter);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        Guard.AgainstEmptyGuid(userId, "User ID cannot be empty.");
        return await repository.GetByIdAsync(userId);
    }
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        Guard.AgainstNullOrEmpty(username, "Username cannot be null or empty.");
        return await repository.GetByUsernameAsync(username);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        Guard.AgainstNullOrEmpty(email, "Email cannot be null or empty.");
        return await repository.GetByEmailAsync(email);
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        Guard.AgainstNull(user, "User cannot be null.");
        Guard.AgainstNullOrEmpty(user.Username, "Username cannot be null or empty.");
        return await repository.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        Guard.AgainstEmptyGuid(userId, "User ID cannot be empty.");
        await tokenService.RevokeAllUserTokensAsync(userId);
        return await repository.DeleteAsync(userId);
    }
    
    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        Guard.AgainstNull(user, "User cannot be null.");
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = 900,
            User = new UserDtoShort
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        };
    }
    
    private static string GetUsernameFromEmail(string email)
    {
        Guard.AgainstNullOrEmpty(email, "Email cannot be null or empty.");
        var atIndex = email.IndexOf('@');
        return atIndex <= 0 ? email : email[..atIndex];
    }
    
    private async Task<string> EnsureUniqueUsernameAsync(string baseUsername)
    {
        Guard.AgainstNullOrEmpty(baseUsername, "Username cannot be null or empty.");
        var username = baseUsername;
        while (await repository.GetByUsernameAsync(username) != null)
        {
            var counter = Random.Shared.Next(1, 1000);
            username = $"{baseUsername}{counter}";
        }
        return username;
    }
}
