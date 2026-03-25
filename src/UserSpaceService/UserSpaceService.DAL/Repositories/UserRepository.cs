using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;

namespace UserSpaceService.DAL.Repositories;

public class UserRepository(UserSpaceDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return null;
        }
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        return user;
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User?> GetByExternalLoginAsync(Provider provider, string providerUserId)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.ExternalLogins.Any(el => el.Provider == provider && el.ProviderKey == providerUserId));
    }

    public async Task<PagedResult<User>> SearchAsync(UserFilter filter)
    {
        var query = context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(u =>
                string.Compare(u.NormalizedUsername, filter.SearchTerm,
                    StringComparison.OrdinalIgnoreCase) == 0);
        }

        query = filter.SortDescending ?
            query.OrderByDescending(u => u.Username) :
            query.OrderBy(u => u.Username);

        var totalCount = await query.CountAsync();
        var pageCount = totalCount / filter.PageSize + (totalCount % filter.PageSize == 0 ? 0 : 1);

        var users = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = users,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            PageCount = pageCount
        };
    }


    public async Task<User> CreateAsync(string username, string email, string passwordHash)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();
        var normalizedUsername = username.Trim().ToUpperInvariant();

        if (await context.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        if (await context.Users.AnyAsync(u => u.NormalizedUsername == normalizedUsername))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            NormalizedEmail = normalizedEmail,
            Username = username.Trim(),
            NormalizedUsername = normalizedUsername,
            PasswordHash = passwordHash,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        context.Users.Add(user);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException p && p.SqlState == "23505")
        {
            throw new InvalidOperationException("Duplicate key violation (email or username).", ex);
        }
        
        return user;
    }
    
    public async Task<User> CreateAsync(string username, string email, Provider provider, string providerUserId)
    {
        var user = new User
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
    
    public async Task<User> AddExternalLoginAsync(User user, Provider provider, string providerUserId)
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

    public async Task<User?> UpdateAsync(User updatedUser)
    {
        var existingUser = await context.Users.FindAsync(updatedUser.Id);
        if (existingUser == null)
        {
            return null;
        }
        context.Entry(existingUser).CurrentValues.SetValues(updatedUser);
        await context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }
}
