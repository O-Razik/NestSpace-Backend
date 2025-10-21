using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Repositories;

public class SpaceRepository(UserSpaceDbContext context) : ISpaceRepository
{
    public async Task<ISpace?> GetByIdAsync(Guid id)
    {
        return await context.Spaces
            .Where(s => s.Id == id)
            .Include(s => s.Members).ThenInclude(m => m.User)
            .Include(s => s.Roles)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ISpace>> GetAllAsync()
    {
        return await context.Spaces.ToListAsync();
    }

    public async Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId)
    {
        return await context.SpaceMembers
            .Where(sm => sm.UserId == userId)
            .Select(sm => sm.Space)
            .ToListAsync();
    }

    public async Task<ISpace> CreateAsync(Guid creatorId, string name)
    {
        var roleId = Guid.NewGuid();
        
        var space = new Space
        {
            Id = Guid.NewGuid(),
            Name = name,
            Roles =
            {
                new SpaceRole
                {
                    Id = roleId,
                    Name = "Owner",
                    RolePermissions = Permission.All,
                }
            },
            Members =
            {
                new SpaceMember
                {
                    Id = Guid.NewGuid(),
                    UserId = creatorId,
                    RoleId = roleId,
                }
            }
        };
        
        context.Spaces.Add(space);
        await context.SaveChangesAsync();
        return space;
    }

    public async Task<ISpace?> UpdateAsync(ISpace entity)
    {
        var space = (Space)entity;
        var existingSpace = await context.Spaces.FindAsync(space.Id);
        if (existingSpace == null)
        {
            return null;
        }
        context.Entry(existingSpace).CurrentValues.SetValues(space);
        await context.SaveChangesAsync();
        return existingSpace;
    }

    public async Task<bool> DeleteAsync(ISpace space)
    {
        context.Spaces.Remove((Space)space);
        await context.SaveChangesAsync();
        return true;
    }
}