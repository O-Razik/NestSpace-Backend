using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public class SpaceMapper(
    IMapper<SpaceMember, SpaceMemberDto> spaceMemberMapper,
    IMapper<SpaceRole, SpaceRoleDto> roleMapper)
    : IMapper<Space, SpaceDto>
{
    public Space ToEntity(SpaceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new Space
        {
            Id = dto.Id,
            Name = dto.Name,
            AvatarUrl = dto.AvatarUrl,
            Members = dto.Members
                .Select(spaceMemberMapper.ToEntity)
                .ToList(),
            Roles = dto.Roles.Select(roleMapper.ToEntity).ToList()
        };
    }

    public SpaceDto ToDto(Space entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            AvatarUrl = entity.AvatarUrl,
            Members = entity.Members
                .Select(spaceMemberMapper.ToDto)
                .ToList(),
            Roles = entity.Roles.Select(roleMapper.ToDto).ToList()
        };
    }
}

public class SpaceShortMapper
    : IMapper<Space, SpaceDtoShort>
{
    public SpaceDtoShort ToDto(Space entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceDtoShort
        {
            Id = entity.Id,
            Name = entity.Name,
            AvatarUrl = entity.AvatarUrl,
        };
    }

    public Space ToEntity(SpaceDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new Space
        {
            Id = dto.Id,
            Name = dto.Name,
            AvatarUrl = dto.AvatarUrl,
        };
    }
}

