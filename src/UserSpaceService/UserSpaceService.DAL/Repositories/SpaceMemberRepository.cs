using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Repositories;

public class SpaceMemberRepository(UserSpaceDbContext context) : ISpaceMemberRepository
{
    public async Task<ISpaceMember?> GetByIdAsync(Guid spaceId, Guid userId)
    {
        return await context.SpaceMembers
            .Include(sm => sm.Role)
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.SpaceId == spaceId && sm.UserId == userId);
    }

    public async Task<ISpaceMember> CreateAsync(Guid spaceId, Guid userId, Guid roleId)
    {
        var spaceMember = new SpaceMember
        {
            Id = Guid.NewGuid(),
            SpaceId = spaceId,
            UserId = userId,
            RoleId = roleId
        };
        
        context.SpaceMembers.Add(spaceMember);
        await context.SaveChangesAsync();
        return spaceMember;
    }

    public async Task<ISpaceMember?> UpdateAsync(ISpaceMember spaceMember)
    {
        var existingSpaceMember = await context.SpaceMembers.FindAsync(spaceMember.Id);
        if (existingSpaceMember == null)
        {
            return null;
        }

        context.Entry(existingSpaceMember).CurrentValues.SetValues(spaceMember);
        await context.SaveChangesAsync();
        return await this.GetByIdAsync(existingSpaceMember.Id, existingSpaceMember.UserId);
    }

    public async Task<bool> DeleteAsync(Guid spaceId, Guid userId)
    {
        var existingSpaceMember = await context.SpaceMembers.FindAsync(spaceId, userId);
        if (existingSpaceMember == null)        {
            return false;
        }
        context.SpaceMembers.Remove(existingSpaceMember);
        await context.SaveChangesAsync();
        return true;
    }
}
