using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.Models;
using UserSpaceService.DAL.Data;

namespace UserSpaceService.DAL.Repositories;

public class SpaceMemberRepository(UserSpaceDbContext context) : ISpaceMemberRepository
{
    public async Task<SpaceMember?> GetByIdAsync(Guid spaceId, Guid userId)
    {
        return await context.SpaceMembers
            .Include(sm => sm.Role)
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.SpaceId == spaceId && sm.UserId == userId);
    }

    public async Task<SpaceMember> CreateAsync(Guid spaceId, Guid userId, Guid roleId)
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

    public async Task<SpaceMember?> UpdateAsync(SpaceMember spaceMember)
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
