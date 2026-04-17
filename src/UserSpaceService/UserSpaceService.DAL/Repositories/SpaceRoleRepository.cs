using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;

namespace UserSpaceService.DAL.Repositories;

public class SpaceRoleRepository(UserSpaceDbContext context) : ISpaceRoleRepository
{
    public async Task<SpaceRole?> GetByIdAsync(Guid roleId)
    {
        return await context.SpaceRoles
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<IEnumerable<SpaceRole>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.SpaceRoles.Where(sr => sr.SpaceId == spaceId).ToListAsync();
    }

    public async Task<SpaceRole> CreateAsync(Guid spaceId, string roleName, Permission permissions)
    {
        var role = new SpaceRole
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            RolePermissions = permissions,
            SpaceId = spaceId
        };
        context.SpaceRoles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    public async Task<SpaceRole?> UpdateAsync(SpaceRole updatedRole)
    {
        var existingRole = await context.SpaceRoles.FindAsync(updatedRole.Id);
        if (existingRole == null)
        {
            return null;
        }
        context.Entry(existingRole).CurrentValues.SetValues(updatedRole);
        await context.SaveChangesAsync();
        return existingRole;
    }

    public async Task<bool> DeleteAsync(Guid roleId)
    {
        var existingRole = await context.SpaceRoles.FindAsync(roleId);
        if (existingRole == null)
        {
            return false;
        }
        
        context.SpaceRoles.Remove(existingRole);
        await context.SaveChangesAsync();
        return true;
    }
}
