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
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            UserId = dto.UserId,
            SpaceUsername = dto.SpaceUsername,
            RoleId = dto.RoleId,
            User = userMapper.ToEntity(dto.User),
            Role = roleMapper.ToEntity(dto.Role)
        };
    }

    public SpaceMemberDto ToDto(SpaceMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceMemberDto
        {
            Id = entity.Id,
            SpaceId = entity.SpaceId,
            UserId = entity.UserId,
            SpaceUsername = entity.SpaceUsername,
            RoleId = entity.RoleId,
            User = userMapper.ToDto(entity.User),
            Role = roleMapper.ToDto(entity.Role),
        };
    }

    public SpaceMember ToEntity(SpaceMemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new SpaceMember
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            UserId = dto.UserId,
            SpaceUsername = dto.SpaceUsername,
            RoleId = dto.RoleId,
        };
    }

    public SpaceMemberDtoShort ToShortDto(SpaceMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceMemberDtoShort
        {
            Id = entity.Id,
            SpaceId = entity.SpaceId,
            UserId = entity.UserId,
            SpaceUsername = entity.SpaceUsername,
            RoleId = entity.RoleId,
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
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            UserId = dto.UserId,
            SpaceUsername = dto.SpaceUsername,
            RoleId = dto.RoleId,
        };
    }

    public SpaceMemberDtoShort ToDto(SpaceMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceMemberDtoShort
        {
            Id = entity.Id,
            SpaceId = entity.SpaceId,
            UserId = entity.UserId,
            SpaceUsername = entity.SpaceUsername,
            RoleId = entity.RoleId,
        };
    }
}
