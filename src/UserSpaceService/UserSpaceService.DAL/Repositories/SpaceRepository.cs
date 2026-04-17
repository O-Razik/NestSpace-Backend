using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.Models;
using UserSpaceService.DAL.Data;

namespace UserSpaceService.DAL.Repositories;

public class SpaceRepository(UserSpaceDbContext context) : ISpaceRepository
{
    public async Task<Space?> GetByIdAsync(Guid spaceId)
    {
        return await context.Spaces
            .Where(s => s.Id == spaceId)
            .Include(s => s.Members)
                .ThenInclude(m => m.User)
            .Include(s => s.Roles)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<Space>> SearchAsync(SpaceFilter filter)
    {
        var query = context.Spaces.AsQueryable();
        
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(s =>
                string.Compare(s.Name, filter.SearchTerm,
                    StringComparison.OrdinalIgnoreCase) == 0);
        }
        
        query = filter.SortBy switch
        {
            SpaceSortBy.NameDescending => query.OrderByDescending(s => s.Name),
            SpaceSortBy.NameAscending => query.OrderBy(s => s.Name),
            SpaceSortBy.MemberCountDescending => query.OrderByDescending(s => s.Members.Count),
            SpaceSortBy.MemberCountAscending => query.OrderBy(s => s.Members.Count),
            _ => query.OrderBy(s => s.Id)
        };
        
        var pageCount = await query.CountAsync() / filter.PageSize
                        + (await query.CountAsync() % filter.PageSize == 0 ? 0 : 1);
        
        var spaces = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(s => s.Members).ThenInclude(m => m.User)
            .Include(s => s.Roles)
            .ToListAsync();
        
        return new PagedResult<Space>
        {
            Items = spaces,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            PageCount = pageCount
        };
    }

    public async Task<Space?> SearchByNameAsync(string name)
    {
        return await context.Spaces
            .Where(s => s.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Space>> GetAllSpacesOfUserAsync(Guid userId)
    {
        return await context.SpaceMembers
            .Where(sm => sm.UserId == userId)
            .Select(sm => sm.Space)
            .ToListAsync();
    }

    public async Task<Space> CreateAsync(Guid creatorId, string name, IList<Guid> memberIds)
    {
        var ownerRoleId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        
        var space = new Space
        {
            Id = Guid.NewGuid(),
            Name = name,
            Roles =
            {
                new SpaceRole
                {
                    Id = ownerRoleId,
                    Name = "Owner",
                    RolePermissions = Permission.All,
                },
                new SpaceRole
                {
                    Id = roleId,
                    Name = "Member",
                    RolePermissions = Permission.ManageTasks | Permission.ManageNotes ,
                }
            },
            Members =
            {
                new SpaceMember
                {
                    Id = Guid.NewGuid(),
                    UserId = creatorId,
                    RoleId = ownerRoleId,
                }
            }
        };
        
        foreach (var memberId in memberIds.Where(memberId => memberId != creatorId))
        {
            space.Members.Add(new SpaceMember
            {
                Id = Guid.NewGuid(),
                UserId = memberId,
                RoleId = roleId,
            });
        }
        
        context.Spaces.Add(space);
        await context.SaveChangesAsync();
        return space;
    }

    public async Task<Space?> UpdateAsync(Space updatedSpace)
    {
        var existingSpace = await context.Spaces.FindAsync(updatedSpace.Id);
        if (existingSpace == null)
        {
            return null;
        }
        context.Entry(existingSpace).CurrentValues.SetValues(updatedSpace);
        await context.SaveChangesAsync();
        return existingSpace;
    }

    public async Task<bool> DeleteAsync(Guid spaceId)
    {
        var space = await context.Spaces.FindAsync(spaceId);
        if (space == null)
        {
            return false;
        }
    
        context.Spaces.Remove(space);
        await context.SaveChangesAsync();
        return true;
    }
}
