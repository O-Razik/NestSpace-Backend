using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.Mappers;

public class SpaceMemberMapper(
    IEntityFactory<ISpaceMember> entityFactory,
    IMapper<IUser, UserDtoShort> userMapper,
    IMapper<ISpaceRole, SpaceRoleDto> roleMapper)
    : IMapper<ISpaceMember, SpaceMemberDto>
{
    public ISpaceMember ToEntity(SpaceMemberDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var spaceMember = entityFactory.CreateEntity();
        spaceMember.Id = dto.Id;
        spaceMember.SpaceId = dto.SpaceId;
        spaceMember.UserId = dto.UserId;
        spaceMember.SpaceUsername = dto.SpaceUsername;
        spaceMember.RoleId = dto.RoleId;
        spaceMember.User = userMapper.ToEntity(dto.User);
        spaceMember.Role = roleMapper.ToEntity(dto.Role);
        return spaceMember;
    }

    public SpaceMemberDto ToDto(ISpaceMember entity)
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

    public ISpaceMember ToEntity(SpaceMemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var spaceMember = entityFactory.CreateEntity();
        spaceMember.Id = dto.Id;
        spaceMember.SpaceId = dto.SpaceId;
        spaceMember.UserId = dto.UserId;
        spaceMember.SpaceUsername = dto.SpaceUsername;
        spaceMember.RoleId = dto.RoleId;
        return spaceMember;
    }

    public SpaceMemberDtoShort ToShortDto(ISpaceMember entity)
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

public class SpaceMemberShortMapper(
    IEntityFactory<ISpaceMember> entityFactory)
    : IMapper<ISpaceMember, SpaceMemberDtoShort>
{
    public ISpaceMember ToEntity(SpaceMemberDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var spaceMember = entityFactory.CreateEntity();
        spaceMember.Id = dto.Id;
        spaceMember.SpaceId = dto.SpaceId;
        spaceMember.UserId = dto.UserId;
        spaceMember.SpaceUsername = dto.SpaceUsername;
        spaceMember.RoleId = dto.RoleId;
        return spaceMember;
    }

    public SpaceMemberDtoShort ToDto(ISpaceMember entity)
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