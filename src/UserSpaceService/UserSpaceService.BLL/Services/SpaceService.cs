using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Queues;
using UserSpaceService.BLL.Queues.Events;

namespace UserSpaceService.BLL.Services;

public class SpaceService(
    ISpaceRepository spaceRepository,
    ISpaceRoleRepository spaceRoleRepository,
    ISpaceMemberRepository spaceMemberRepository,
    IUserRepository userRepository,
    IEventPublisher eventPublisher)
    : ISpaceService
{
    public async Task<PagedResult<ISpace>> SearchSpacesAsync(SpaceFilter filter)
    {
        return await spaceRepository.SearchAsync(filter);
    }

    public async Task<IEnumerable<ISpace>> GetAllSpacesOfUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId) 
                    ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        return await spaceRepository.GetAllSpacesOfUserAsync(user.Id);
    }

    public async Task<ISpace> GetSpaceByIdAsync(Guid spaceId)
    {
        var space = await spaceRepository.GetByIdAsync(spaceId) 
                    ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");
        return space;
    }

    public async Task<ISpace> CreateSpaceAsync(Guid creatorId, string name, List<Guid> memberIds)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Space name cannot be null or empty.", nameof(name));
        }

        if (await spaceRepository.SearchByNameAsync(name) != null)
        {
            throw new InvalidOperationException($"A space with the name '{name}' already exists.");
        }

        var creator = await userRepository.GetByIdAsync(creatorId)
                      ?? throw new InvalidOperationException("Creator user ID is not available.");
        
        foreach (var memberId in memberIds)
        {
            var member = await userRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {memberId} not found.");
            }
        }

        var space = await spaceRepository.CreateAsync(creator.Id, name, memberIds);
        
        var chatEvent = new ChatCreateEvent
        {
            SpaceId = space.Id,
            ChatId = Guid.NewGuid(),
            MemberId = creatorId,
            CreatedAt = DateTime.UtcNow,
        };
        
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = space.Id,
            MemberId = creator.Id,
            Type = "SpaceCreated",
            Description = $"Space '{name}' created by user '{creator.Username}'.",
            ActivityAt = DateTime.UtcNow
        };
        
        await eventPublisher.PublishAsync(
            logEvent,
            routingKey: "space.activity.log",
            exchangeName: "log.exchange"
        );

        await eventPublisher.PublishAsync(
            chatEvent,
            routingKey: "chat.create",
            exchangeName: "chat.exchange"
        );
        
        return space;
    }

    public async Task<ISpace?> UpdateSpaceNameAsync(Guid spaceId, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("New space name cannot be null or empty.", nameof(newName));
        }

        var space = await spaceRepository.GetByIdAsync(spaceId) 
                    ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");

        if ((await spaceRepository.SearchByNameAsync(newName)) != null)
        {
            throw new InvalidOperationException($"A space with the name '{newName}' already exists.");
        }

        space.Name = newName;
        var result = await spaceRepository.UpdateAsync(space);

        if (result != null)
        {
            var logEvent = new SpaceActivityLogEvent
            {
                SpaceId = space.Id,
                MemberId = Guid.Empty, // System action
                Type = "SpaceNameUpdated",
                Description = $"Space name updated to '{newName}'.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );

            return space;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> DeleteSpaceAsync(Guid spaceId)
    {
        if (spaceId == Guid.Empty)
        {
            throw new ArgumentException("Space ID cannot be empty.", nameof(spaceId));
        }

        var result = await spaceRepository.DeleteAsync(spaceId);

        if (result)
        {
            var deleteEvent = new DeleteSpaceEvent
            {
                SpaceId = spaceId,
                DeletedAt = DateTime.UtcNow
            };

            await eventPublisher.PublishAsync(
                deleteEvent,
                routingKey: "space.deleted",
                exchangeName: "space.exchange"
            );
        }
        
        return result;
    }

    public async Task<ISpaceRole> CreateSpaceRoleAsync(Guid spaceId, string roleName, Permission permissions)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));

        var space = await spaceRepository.GetByIdAsync(spaceId)
                    ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");

        if (space.Roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A role with the name '{roleName}' already exists in this space.");
        }
        
        var result = await spaceRoleRepository.CreateAsync(spaceId, roleName, permissions);
        
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = space.Id,
            MemberId = Guid.Empty, // System action
            Type = "SpaceRoleCreated",
            Description = $"Role '{roleName}' created in space '{space.Name}'.",
            ActivityAt = DateTime.UtcNow
        };
        
        await eventPublisher.PublishAsync(
            logEvent,
            routingKey: "space.activity.log",
            exchangeName: "log.exchange"
        );
        
        return result;
    }

    public async Task<ISpaceRole?> UpdateSpaceRoleAsync(ISpaceRole spaceRole)
    {
        if (spaceRole == null)
            throw new ArgumentNullException(nameof(spaceRole), "Space role cannot be null.");

        if (spaceRole.Id == Guid.Empty)
            throw new ArgumentException("Space role ID cannot be empty.", nameof(spaceRole));

        var existingRole = await spaceRoleRepository.GetByIdAsync(spaceRole.Id)
                            ?? throw new KeyNotFoundException($"Space role with ID {spaceRole.Id} not found.");

        if (string.IsNullOrWhiteSpace(spaceRole.Name))
            throw new ArgumentException("Role name cannot be null or empty.", nameof(spaceRole));

        existingRole.Name = spaceRole.Name;
        existingRole.RolePermissions = spaceRole.RolePermissions;
        
        var result = await spaceRoleRepository.UpdateAsync(existingRole);
        
        if (result != null)
        {
            var logEvent = new SpaceActivityLogEvent
            {
                SpaceId = existingRole.SpaceId,
                MemberId = Guid.Empty, // System action
                Type = "SpaceRoleUpdated",
                Description = $"Role '{spaceRole.Name}' updated in space ID '{existingRole.SpaceId}'.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );

            return result;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> DeleteSpaceRoleAsync(Guid spaceId, Guid roleId)
    {
        if (spaceId == Guid.Empty)
            throw new ArgumentException("Space ID cannot be empty.", nameof(spaceId));

        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));

        var space = await spaceRepository.GetByIdAsync(spaceId)
                        ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");

        var role = await spaceRoleRepository.GetByIdAsync(roleId);
        if (role == null || role.SpaceId != space.Id)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found in space {spaceId}.");
        }

        var result = await spaceRoleRepository.DeleteAsync(role);

        if (!result) return result;
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = space.Id,
            MemberId = Guid.Empty, // System action
            Type = "SpaceRoleDeleted",
            Description = $"Role '{role.Name}' deleted from space '{space.Name}'.",
            ActivityAt = DateTime.UtcNow
        };
            
        await eventPublisher.PublishAsync(
            logEvent,
            routingKey: "space.activity.log",
            exchangeName: "log.exchange"
        );

        return result;
    }

    public async Task<ISpaceMember> AddMemberToSpaceAsync(Guid spaceId, Guid userId, Guid roleId)
    {
        if (spaceId == Guid.Empty)
            throw new ArgumentException("Space ID cannot be empty.", nameof(spaceId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));

        var space = await spaceRepository.GetByIdAsync(spaceId) 
                    ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");
        var user = await userRepository.GetByIdAsync(userId) 
                    ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        var role = await spaceRoleRepository.GetByIdAsync(roleId);
        if (role == null || role.SpaceId != spaceId)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found in space {spaceId}.");
        }

        var existingMember = await spaceMemberRepository.GetByIdAsync(space.Id, user.Id);
        
        if (existingMember != null)
            throw new InvalidOperationException($"User with ID {userId} is already a member of space {spaceId}.");

        var result = await spaceMemberRepository.CreateAsync(spaceId, userId, roleId);
        
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = space.Id,
            MemberId = user.Id,
            Type = "MemberAdded",
            Description = $"User '{user.Username}' added to space '{space.Name}' with role '{role.Name}'.",
            ActivityAt = DateTime.UtcNow
        };
        
        await eventPublisher.PublishAsync(
            logEvent,
            routingKey: "space.activity.log",
            exchangeName: "log.exchange"
        );
        
        return result;
    }

    public async Task<ISpaceMember?> UpdateSpaceMemberAsync(ISpaceMember spaceMember)
    {
        if (spaceMember == null)
            throw new ArgumentNullException(nameof(spaceMember), "Space member cannot be null.");

        if (spaceMember.Id == Guid.Empty)
            throw new ArgumentException("Space member ID cannot be empty.", nameof(spaceMember));

        var existingMember = await spaceMemberRepository.GetByIdAsync(spaceMember.SpaceId, spaceMember.UserId)
                            ?? throw new KeyNotFoundException($"Member with User ID {spaceMember.UserId} not found in space {spaceMember.SpaceId}.");

        existingMember.RoleId = spaceMember.RoleId;
        existingMember.SpaceUsername = spaceMember.SpaceUsername;

        var result = await spaceMemberRepository.UpdateAsync(existingMember);
        
        if (result != null)
        {
            var logEvent = new SpaceActivityLogEvent
            {
                SpaceId = existingMember.SpaceId,
                MemberId = existingMember.UserId,
                Type = "SpaceMemberUpdated",
                Description = $"Member with User ID '{spaceMember.UserId}' updated in space ID '{existingMember.SpaceId}'.",
                ActivityAt = DateTime.UtcNow
            };
            
            await eventPublisher.PublishAsync(
                logEvent,
                routingKey: "space.activity.log",
                exchangeName: "log.exchange"
            );

            return result;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> RemoveMemberFromSpaceAsync(Guid spaceId, Guid userId)
    {
        if (spaceId == Guid.Empty)
        {
            throw new ArgumentException("Space ID cannot be empty.", nameof(spaceId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        var space = await spaceRepository.GetByIdAsync(spaceId)
                        ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");

        var member = await spaceMemberRepository.GetByIdAsync(space.Id, userId)
                        ?? throw new KeyNotFoundException($"Member with User ID {userId} not found in space {spaceId}.");

        var result= await spaceMemberRepository.DeleteAsync(member);
        
        if (!result)
        {
            return result;
        }
        
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = space.Id,
            MemberId = member.UserId,
            Type = "MemberRemoved",
            Description = $"User with ID '{member.UserId}' removed from space '{space.Name}'.",
            ActivityAt = DateTime.UtcNow,
        };
        
        await eventPublisher.PublishAsync(
            logEvent,
            routingKey: "space.activity.log",
            exchangeName: "log.exchange"
        );
        
        return result;
    }
}

