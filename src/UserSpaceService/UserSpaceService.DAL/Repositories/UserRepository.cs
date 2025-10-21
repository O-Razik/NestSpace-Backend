using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Repositories;

public class UserRepository(UserSpaceDbContext context) : IUserRepository
{
    public async Task<IUser?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return null;
        }
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
        
        return user;
    }
    
    public async Task<IUser?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<IUser?> GetByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<IUser?> GetByExternalLoginAsync(Provider provider, string providerUserId)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.ExternalLogins.Any(el => el.Provider == provider && el.ProviderKey == providerUserId));
    }

    public async Task<IEnumerable<IUser>> GetAllAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<IUser> CreateAsync(string username, string email, string passwordHash)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            NormalizedUsername = username.ToUpperInvariant(),
            NormalizedEmail = email.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
    
    public async Task<IUser> CreateAsync(string username, string email, Provider provider, string providerUserId)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            Username = username,
            NormalizedUsername = username.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ExternalLogins = new List<ExternalLogin>
            {
                new ExternalLogin
                {
                    Provider = provider,
                    ProviderKey = providerUserId,
                }
            }
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
    
    public async Task<IUser> AddExternalLoginAsync(IUser user, Provider provider, string providerUserId)
    {
        var existingUser = await context.Users
            .Include(u => u.ExternalLogins)
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        
        if (existingUser == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        if (existingUser.ExternalLogins.Any(el => el.Provider == provider && el.ProviderKey == providerUserId))
        {
            return existingUser;
        }
        
        existingUser.ExternalLogins.Add(new ExternalLogin
        {
            Provider = provider,
            ProviderKey = providerUserId
        });
        
        await context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<IUser?> UpdateAsync(IUser entity)
    {
        var user = (User)entity;
        var existingUser = await context.Users.FindAsync(user.Id);
        if (existingUser == null)
        {
            return null;
        }
        context.Entry(existingUser).CurrentValues.SetValues(user);
        await context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteAsync(IUser user)
    {
        context.Users.Remove((User)user);
        await context.SaveChangesAsync();
        return true;
    }
}