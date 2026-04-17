using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.BLL.Services;

public class UserService(
    IUserRepository repository,
    PasswordService passwordService,
    ITokenService tokenService)
    : IUserService
{
    private const int AccessTokenExpirySeconds = 900;

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var passwordHash = passwordService.HashPassword(registerDto.Password);
        var user = await repository.CreateAsync(registerDto.Username, registerDto.Email, passwordHash);
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RegisterByExternalProviderAsync(Provider provider, string providerUserId, string email)
    {
        Guard.AgainstNullOrEmpty(providerUserId);
        Guard.AgainstNullOrEmpty(email);
        
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
        Guard.AgainstNullOrEmpty(providerUserId);
        var user = await repository.GetByExternalLoginAsync(provider, providerUserId);
        return user == null ?  null : await GenerateAuthResponseAsync(user);
    }
    
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        Guard.AgainstNullOrEmpty(refreshToken);

        var token = await tokenService.GetRefreshTokenAsync(refreshToken);
        Guard.AgainstNull(token);

        if(!string.IsNullOrEmpty(token.ReplacedByToken))
        {
            throw new UnauthorizedAccessException("Refresh token has been replaced by a newer token.");
        }
        if (!tokenService.IsRefreshTokenValid(token))
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var newRefreshTokenValue = tokenService.GenerateRefreshToken();

        await tokenService.RevokeRefreshTokenAsync(refreshToken, newRefreshTokenValue);
        await tokenService.SaveRefreshTokenAsync(token.UserId, newRefreshTokenValue);

        var accessToken = tokenService.GenerateAccessToken(token.User);

        return new AuthResponseDto {
            AccessToken = accessToken,
            RefreshToken = newRefreshTokenValue,
            TokenType = "Bearer",
            ExpiresIn = AccessTokenExpirySeconds,
            User = new UserDtoShort {
                Id = token.User.Id,
                Username = token.User.Username,
                Email = token.User.Email }
        };
    }
    
    public async Task LogoutAsync(string refreshToken)
    {
        Guard.AgainstNullOrEmpty(refreshToken);
        await tokenService.RevokeRefreshTokenAsync(refreshToken);
    }

    public async Task<User?> AddExternalLoginAsync(Guid userId, Provider provider, string providerUserId)
    {
        Guard.AgainstEmptyGuid(userId);
        Guard.AgainstNullOrEmpty(providerUserId);

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
        Guard.AgainstEmptyGuid(userId);
        return await repository.GetByIdAsync(userId);
    }
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        Guard.AgainstNullOrEmpty(username);
        return await repository.GetByUsernameAsync(username);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        Guard.AgainstNullOrEmpty(email);
        return await repository.GetByEmailAsync(email);
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        Guard.AgainstNull(user);
        Guard.AgainstNullOrEmpty(user.Username);
        return await repository.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        Guard.AgainstEmptyGuid(userId);
        await tokenService.RevokeAllUserTokensAsync(userId);
        return await repository.DeleteAsync(userId);
    }
    
    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        Guard.AgainstNull(user);
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = AccessTokenExpirySeconds,
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
        Guard.AgainstNullOrEmpty(email);
        var atIndex = email.IndexOf('@');
        return atIndex <= 0 ? email : email[..atIndex];
    }
    
    private async Task<string> EnsureUniqueUsernameAsync(string baseUsername)
    {
        Guard.AgainstNullOrEmpty(baseUsername);
        var username = baseUsername;
        while (await repository.GetByUsernameAsync(username) != null)
        {
            var counter = Random.Shared.Next(1, 1000);
            username = $"{baseUsername}{counter}";
        }
        return username;
    }
}
