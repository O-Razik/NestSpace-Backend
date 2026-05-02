using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public static class SpaceMemberMapper
{
    public static SpaceMember ToEntity(this SpaceMemberDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new SpaceMember
        {
            SpaceId = dto.SpaceId,
            UserId = dto.UserId,
            SpaceUsername = dto.SpaceUsername,
            RoleId = dto.RoleId,
            SubgroupId = dto.SubgroupId,
            JoinedAt = dto.JoinedAt,
            User = dto.User.ToEntity(),
            Role = dto.Role.ToEntity()
        };
    }
    
    public static SpaceMember ToEntity(this SpaceMemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new SpaceMember
        {
            SpaceId = dto.SpaceId,
            UserId = dto.UserId,
            SpaceUsername = dto.SpaceUsername,
            RoleId = dto.RoleId,
            SubgroupId = dto.SubgroupId,
            JoinedAt = dto.JoinedAt
        };
    }
    
    public static SpaceMember ShortToEntity(this SpaceMemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new SpaceMember
        {
            SpaceId = dto.SpaceId,
            UserId = dto.UserId,
            SpaceUsername = dto.SpaceUsername,
            RoleId = dto.RoleId,
            SubgroupId = dto.SubgroupId,
            JoinedAt = dto.JoinedAt
        };
    }

    public static SpaceMemberDto ToDto(this SpaceMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceMemberDto
        {
            SpaceId = entity.SpaceId,
            UserId = entity.UserId,
            SpaceUsername = entity.SpaceUsername,
            RoleId = entity.RoleId,
            SubgroupId = entity.SubgroupId,
            JoinedAt = entity.JoinedAt,
            User = entity.User.ToDto(),
            Role = entity.Role.ToDto(),
        };
    }

    public static SpaceMemberDtoShort ToShortDto(this SpaceMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceMemberDtoShort
        {
            SpaceId = entity.SpaceId,
            UserId = entity.UserId,
            SpaceUsername = entity.SpaceUsername,
            RoleId = entity.RoleId,
            SubgroupId = entity.SubgroupId,
            JoinedAt = entity.JoinedAt
        };
    }
}
