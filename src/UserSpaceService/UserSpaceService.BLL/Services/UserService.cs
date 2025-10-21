using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;

namespace UserSpaceService.BLL.Services;

public class UserService(
    IUserRepository repository,
    IConfiguration configuration,
    PasswordService passwordService)
    : IUserService
{

    public async Task<string> RegisterAsync(string username, string email, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }
        
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }
        
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }
        
        var passwordHash = passwordService.HashPassword(password);
        var user = await repository.CreateAsync(username, email, passwordHash);
        
        if (user == null)
        {
            throw new Exception("User registration failed.");
        }
        
        return GenerateJwtTokenAsync(user, TimeSpan.FromDays(7));
    }

    public async Task<string> RegisterByExternalProviderAsync(Provider provider, string providerUserId, string email)
    {
        
        if (string.IsNullOrEmpty(providerUserId))
        {
            throw new ArgumentException("Provider user ID cannot be null or empty.", nameof(providerUserId));
        }
        
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }
        
        var baseUsername = GetUsernameFromEmail(email);
        var uniqueUsername = await EnsureUniqueUsername(baseUsername);
        var user = await repository.CreateAsync(uniqueUsername, email, provider, providerUserId);
        
        return GenerateJwtTokenAsync(user, TimeSpan.FromDays(7));
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }
        
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        var user = await repository.GetByEmailAsync(email);
        if (user == null)
        {
            return null; // User not found
        }

        var valid = passwordService.VerifyPassword(password, user.PasswordHash);
        return !valid ? null : GenerateJwtTokenAsync(user, TimeSpan.FromDays(7));
    }

    public async Task<string?> LoginByExternalProviderAsync(Provider provider, string providerUserId)
    {
        if (string.IsNullOrEmpty(providerUserId))
        {
            throw new ArgumentException("Provider user ID cannot be null or empty.", nameof(providerUserId));
        }

        var user = await repository.GetByExternalLoginAsync(provider, providerUserId);
        return user == null ? null : GenerateJwtTokenAsync(user, TimeSpan.FromDays(7));
    }

    public async Task<IUser?> AddExternalLoginAsync(Guid userId, Provider provider, string providerUserId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (string.IsNullOrEmpty(providerUserId))
            throw new ArgumentException("Provider user ID cannot be null or empty.", nameof(providerUserId));

        var user = await repository.GetByIdAsync(userId);
        if (user == null)
            return null;

        // Prevent duplicate external login
        if (user.ExternalLogins.Any(el => el.Provider == provider && el.ProviderKey == providerUserId))
            return user;
        
        await repository.AddExternalLoginAsync(user, provider, providerUserId);
        
        return user;
    }

    public async Task<IUser?> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty) 
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
        
        return await repository.GetByIdAsync(userId);
    }
    
    public async Task<IUser?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }
        
        return await repository.GetByUsernameAsync(username);
    }
    
    public async Task<IUser?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }
        
        return await repository.GetByEmailAsync(email);
    }

    public async Task<IUser?> UpdateUserAsync(IUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        if (user.Id == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(user));
        }

        return await repository.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await this.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }
        return await repository.DeleteAsync(user);
    }

    public string GenerateJwtTokenAsync(IUser user, TimeSpan expiration)
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
                new Claim(ClaimTypes.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.Add(expiration),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private static string GetUsernameFromEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        var atIndex = email.IndexOf('@');
        return atIndex <= 0 ? email : email[..atIndex];
    }
    
    private async Task<string> EnsureUniqueUsername(string baseUsername)
    {
        var username = baseUsername;
        while (await repository.GetByUsernameAsync(username) != null)
        {
            var counter = Random.Shared.Next(1, 1000);
            username = $"{baseUsername}{counter}";
        }
        return username;
    }
}