using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public class SpaceMemberMapper(
    IMapper<User, UserDtoShort> userMapper,
    IMapper<SpaceRole, SpaceRoleDto> roleMapper)
    : IMapper<SpaceMember, SpaceMemberDto>
{
    public SpaceMember ToEntity(SpaceMemberDto dto)
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
            User = userMapper.ToEntity(dto.User),
            Role = roleMapper.ToEntity(dto.Role)
        };
    }
    
    public SpaceMember ToEntity(SpaceMemberDtoShort dto)
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

    public SpaceMemberDto ToDto(SpaceMember entity)
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
            User = userMapper.ToDto(entity.User),
            Role = roleMapper.ToDto(entity.Role),
        };
    }

    public SpaceMemberDtoShort ToShortDto(SpaceMember entity)
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

public class SpaceMemberShortMapper
    : IMapper<SpaceMember, SpaceMemberDtoShort>
{
    public SpaceMember ToEntity(SpaceMemberDtoShort dto)
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

    public SpaceMemberDtoShort ToDto(SpaceMember entity)
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
