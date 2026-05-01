using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.Models;
using UserSpaceService.DAL.Data;

namespace UserSpaceService.DAL.Repositories;

public class SpaceMemberRepository(UserSpaceDbContext context, IDateTimeProvider dateTimeProvider) : ISpaceMemberRepository
{
    public async Task<SpaceMember?> GetByIdAsync(Guid spaceId, Guid userId)
    {
        return await context.SpaceMembers
            .Include(sm => sm.Role)
            .Include(sm => sm.User)
            .Include(sm => sm.Subgroup)
            .FirstOrDefaultAsync(sm => sm.SpaceId == spaceId && sm.UserId == userId);
    }

    public async Task<SpaceMember> CreateAsync(Guid spaceId, Guid userId, Guid roleId)
    {
        var spaceMember = new SpaceMember
        {
            SpaceId = spaceId,
            UserId = userId,
            RoleId = roleId,
            JoinedAt = dateTimeProvider.UtcNow
        };

        context.SpaceMembers.Add(spaceMember);
        await context.SaveChangesAsync();
        return spaceMember;
    }

    public async Task<SpaceMember?> UpdateAsync(SpaceMember spaceMember)
    {
        // 🔧 Composite Key: (SpaceId, UserId) не (Id)
        var existingSpaceMember = await context.SpaceMembers.FindAsync(spaceMember.SpaceId, spaceMember.UserId);
        if (existingSpaceMember == null)
        {
            return null;
        }

        existingSpaceMember.RoleId = spaceMember.RoleId;
        existingSpaceMember.SpaceUsername = spaceMember.SpaceUsername;
        existingSpaceMember.SubgroupId = spaceMember.SubgroupId;

        context.SpaceMembers.Update(existingSpaceMember);
        await context.SaveChangesAsync();

        return await this.GetByIdAsync(existingSpaceMember.SpaceId, existingSpaceMember.UserId);
    }

    public async Task<bool> DeleteAsync(Guid spaceId, Guid userId)
    {
        var existingSpaceMember = await context.SpaceMembers.FindAsync(spaceId, userId);
        if (existingSpaceMember == null)
        {
            return false;
        }
        context.SpaceMembers.Remove(existingSpaceMember);
        await context.SaveChangesAsync();
        return true;
    }
}
