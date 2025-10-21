using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Repositories;

public class SpaceRoleRepository(UserSpaceDbContext context) : ISpaceRoleRepository
{
    public async Task<ISpaceRole?> GetByIdAsync(Guid id)
    {
        return await context.SpaceRoles
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<ISpaceRole>> GetBySpaceAsync(Guid spaceId)
    {
        return await context.SpaceRoles.Where(sr => sr.SpaceId == spaceId).ToListAsync();
    }

    public async Task<ISpaceRole> CreateAsync(Guid spaceId, string roleName, Permission permissions)
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

    public async Task<ISpaceRole?> UpdateAsync(ISpaceRole entity)
    {
        var role = (SpaceRole)entity;
        var existingRole = await context.SpaceRoles.FindAsync(role.Id);
        if (existingRole == null)
        {
            return null;
        }
        context.Entry(existingRole).CurrentValues.SetValues(role);
        await context.SaveChangesAsync();
        return existingRole;
    }

    public async Task<bool> DeleteAsync(ISpaceRole spaceRole)
    {
        context.SpaceRoles.Remove((SpaceRole)spaceRole);
        await context.SaveChangesAsync();
        return true;
    }
}