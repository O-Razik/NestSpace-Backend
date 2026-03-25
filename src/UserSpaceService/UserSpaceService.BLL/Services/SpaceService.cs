using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.Filters;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.IServices;
using UserSpaceService.BLL.Helpers;
using UserSpaceService.BLL.Queues.Events;

namespace UserSpaceService.BLL.Services;

public class SpaceService(
    ISpaceRepository spaceRepository,
    ISpaceRoleRepository spaceRoleRepository,
    ISpaceMemberRepository spaceMemberRepository,
    IUserRepository userRepository,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
    : ISpaceService
{
    private const string RoutingKeyPrefix = "space";

    public async Task<PagedResult<Space>> SearchSpacesAsync(SpaceFilter filter)
    {
        return await spaceRepository.SearchAsync(filter);
    }

    public async Task<IEnumerable<Space>> GetAllSpacesOfUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId) 
                    ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        return await spaceRepository.GetAllSpacesOfUserAsync(user.Id);
    }

    public async Task<Space?> GetSpaceByIdAsync(Guid spaceId)
    {
        var space = await spaceRepository.GetByIdAsync(spaceId);
        return space;
    }

    public async Task<Space> CreateSpaceAsync(CreateSpaceDto createSpaceDto)
    {
        if (await spaceRepository.SearchByNameAsync(createSpaceDto.Name) != null)
        {
            throw new InvalidOperationException($"A space with the name '{createSpaceDto.Name}' already exists.");
        }

        var creator = await userRepository.GetByIdAsync(createSpaceDto.CreatorId)
                      ?? throw new InvalidOperationException("Creator user ID is not available.");
        
        foreach (var memberId in createSpaceDto.MemberIds)
        {
            var member = await userRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {memberId} not found.");
            }
        }

        var space = await spaceRepository.CreateAsync(creator.Id, createSpaceDto.Name, [..createSpaceDto.MemberIds]);
        
        var chatEvent = new ChatCreateEvent
        {
            SpaceId = space.Id,
            ChatId = Guid.NewGuid(),
            MemberId = createSpaceDto.CreatorId,
            CreatedAt = dateTimeProvider.UtcNow
        };
        
        await eventPublisher.PublishAsync(
            chatEvent,
            routingKey: "chat.create",
            exchangeName: "chat.exchange"
        );
        
        await PublishSpaceActivityLogAsync(
            space.Id, creator.Id,
            "SpaceCreated",
            $"Space '{space.Name}' created by user '{creator.Username}'.");
        
        return space;
    }

    public async Task<Space?> UpdateSpaceNameAsync(Guid spaceId, string newName, Guid memberId)
    {
        Guard.AgainstNullOrWhiteSpace(newName, "New space name cannot be null or empty.");
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
            await PublishSpaceActivityLogAsync(
                space.Id, memberId,
                "SpaceNameUpdated",
                $"Space name updated to '{newName}'.");
        }

        return space;
    }

    public async Task<bool> DeleteSpaceAsync(Guid spaceId)
    {
        Guard.AgainstEmptyGuid(spaceId, "Space ID cannot be null or empty.");
        var result = await spaceRepository.DeleteAsync(spaceId);

        if (result)
        {
            var deleteEvent = new DeleteSpaceEvent
            {
                SpaceId = spaceId,
                DeletedAt = dateTimeProvider.UtcNow
            };

            await eventPublisher.PublishAsync(
                deleteEvent,
                routingKey: RoutingKeyPrefix + ".deleted",
                exchangeName: "space.exchange"
            );
        }
        
        return result;
    }

    public async Task<SpaceRole> CreateSpaceRoleAsync(
        Guid spaceId, string roleName,
        Permission permissions, Guid memberId)
    {
        Guard.AgainstNullOrWhiteSpace(roleName, "Role name cannot be null or empty.");
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
            MemberId = memberId,
            Type = "SpaceRoleCreated",
            Description = $"Role '{roleName}' created in space '{space.Name}'.",
            ActivityAt = dateTimeProvider.UtcNow,
        };
        
        await eventPublisher.PublishAsync(
            logEvent,
            routingKey: RoutingKeyPrefix +".activity.log",
            exchangeName: "log.exchange"
        );
        
        return result;
    }

    public async Task<SpaceRole?> UpdateSpaceRoleAsync(SpaceRole spaceRole, Guid memberId)
    {
        Guard.AgainstNull(spaceRole, "Space role cannot be null.");
        Guard.AgainstEmptyGuid(spaceRole.Id, "Space role ID cannot be empty.");

        var existingRole = await spaceRoleRepository.GetByIdAsync(spaceRole.Id)
                            ?? throw new KeyNotFoundException($"Space role with ID {spaceRole.Id} not found.");
        
        Guard.AgainstNullOrWhiteSpace(spaceRole.Name, "Role name cannot be null or empty.");
        existingRole.Name = spaceRole.Name;
        existingRole.RolePermissions = spaceRole.RolePermissions;
        
        var result = await spaceRoleRepository.UpdateAsync(existingRole);
        
        if (result != null)
        {
            await PublishSpaceActivityLogAsync(
                existingRole.SpaceId, memberId,
                "SpaceRoleUpdated",
                $"Role '{spaceRole.Name}' updated in space ID '{existingRole.SpaceId}'.");
            
        }
        
        return result;
    }

    public async Task<bool> DeleteSpaceRoleAsync(Guid spaceId, Guid roleId, Guid memberId)
    {
        Guard.AgainstEmptyGuid(spaceId, "Space ID cannot be empty.");
        Guard.AgainstEmptyGuid(roleId, "Role ID cannot be empty.");

        var space = await spaceRepository.GetByIdAsync(spaceId)
                        ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");

        var role = await spaceRoleRepository.GetByIdAsync(roleId);
        if (role == null || role.SpaceId != space.Id)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found in space {spaceId}.");
        }

        var result = await spaceRoleRepository.DeleteAsync(roleId);
        
        if (result)
        {
            await PublishSpaceActivityLogAsync(
                space.Id, memberId,
                "SpaceRoleDeleted",
                $"Role '{role.Name}' deleted from space '{space.Name}'.");
        }

        return result;
    }

    public async Task<SpaceMember> AddMemberToSpaceAsync(Guid spaceId, Guid userId, Guid roleId)
    {
        Guard.AgainstEmptyGuid(spaceId, "Space ID cannot be empty.");
        Guard.AgainstEmptyGuid(userId, "User ID cannot be empty.");
        Guard.AgainstEmptyGuid(roleId, "Role ID cannot be empty.");

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
        {
            throw new InvalidOperationException($"User with ID {userId} is already a member of space {spaceId}.");
        }

        var result = await spaceMemberRepository.CreateAsync(spaceId, userId, roleId);
        
        await PublishSpaceActivityLogAsync(
            space.Id, user.Id,
            "MemberAdded",
            $"User '{user.Username}' added to space '{space.Name}' with role '{role.Name}'.");
        
        return result;
    }

    public async Task<SpaceMember?> UpdateSpaceMemberAsync(SpaceMember spaceMember)
    {
        Guard.AgainstNull(spaceMember, "Space member cannot be null.");
        Guard.AgainstEmptyGuid(spaceMember.Id, "Space member cannot be empty.");

        var existingMember = await spaceMemberRepository.GetByIdAsync(spaceMember.SpaceId, spaceMember.UserId)
                            ?? throw new KeyNotFoundException($"Member with User ID {spaceMember.UserId} not found in space {spaceMember.SpaceId}.");

        existingMember.RoleId = spaceMember.RoleId;
        existingMember.SpaceUsername = spaceMember.SpaceUsername;

        var result = await spaceMemberRepository.UpdateAsync(existingMember);
        
        if (result != null)
        {
            await PublishSpaceActivityLogAsync(
                existingMember.SpaceId,
                existingMember.UserId,
                "MemberUpdated",
                $"User with ID '{existingMember.UserId}' updated in space ID '{existingMember.SpaceId}'.");
        }
        return result;
    }

    public async Task<bool> RemoveMemberFromSpaceAsync(Guid spaceId, Guid userId)
    {
        Guard.AgainstEmptyGuid(spaceId, "Space ID cannot be empty.");
        Guard.AgainstEmptyGuid(userId, "User ID cannot be empty.");

        var space = await spaceRepository.GetByIdAsync(spaceId)
                        ?? throw new KeyNotFoundException($"Space with ID {spaceId} not found.");

        var member = await spaceMemberRepository.GetByIdAsync(space.Id, userId)
                        ?? throw new KeyNotFoundException($"Member with User ID {userId} not found in space {spaceId}.");

        var result= await spaceMemberRepository.DeleteAsync(spaceId, userId);
        
        if (result)
        {
            await PublishSpaceActivityLogAsync(
                space.Id, member.UserId,
                "MemberRemoved",
                $"User with ID '{member.UserId}' removed from space '{space.Name}'.");
        }
        
        return result;
    }
    
    private Task PublishSpaceActivityLogAsync(Guid spaceId, Guid memberId, string type, string description)
    {
        var logEvent = new SpaceActivityLogEvent
        {
            SpaceId = spaceId,
            MemberId = memberId,
            Type = type,
            Description = description,
            ActivityAt = dateTimeProvider.UtcNow
        };
        
        return eventPublisher.PublishAsync(
            logEvent,
            routingKey: RoutingKeyPrefix +".activity.log",
            exchangeName: "log.exchange"
        );
    }
}

