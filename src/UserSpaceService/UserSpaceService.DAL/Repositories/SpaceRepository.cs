using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Repositories;

public class SpaceRepository(UserSpaceDbContext context) : ISpaceRepository
{
    public async Task<ISpace?> GetByIdAsync(Guid spaceId)
    {
        return await context.Spaces
            .Where(s => s.Id == spaceId)
            .Include(s => s.Members)
                .ThenInclude(m => m.User)
            .Include(s => s.Roles)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<ISpace>> SearchAsync(SpaceFilter filter)
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
        
        return new PagedResult<ISpace>
        {
            Items = spaces,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            PageCount = pageCount
        };
    }

    public async Task<ISpace?> SearchByNameAsync(string name)
    {
        return await context.Spaces
            .Where(s => s.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId)
    {
        return await context.SpaceMembers
            .Where(sm => sm.UserId == userId)
            .Select(sm => sm.Space)
            .ToListAsync();
    }

    public async Task<ISpace> CreateAsync(Guid creatorId, string name, List<Guid> memberIds)
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

    public async Task<ISpace?> UpdateAsync(ISpace updatedSpace)
    {
        var space = (Space)updatedSpace;
        var existingSpace = await context.Spaces.FindAsync(space.Id);
        if (existingSpace == null)
        {
            return null;
        }
        context.Entry(existingSpace).CurrentValues.SetValues(space);
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