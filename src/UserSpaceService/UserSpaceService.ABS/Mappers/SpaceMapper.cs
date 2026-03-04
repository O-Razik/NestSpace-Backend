using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.Mappers;

public class SpaceMapper(
    IEntityFactory<ISpace> entityFactory,
    IMapper<ISpaceMember, SpaceMemberDto> spaceMemberMapper,
    IMapper<ISpaceRole, SpaceRoleDto> roleMapper)
    : IMapper<ISpace, SpaceDto>
{
    public ISpace ToEntity(SpaceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var space = entityFactory.CreateEntity();
        space.Id = dto.Id;
        space.Name = dto.Name;
        space.Members = dto.Members
            .Select(spaceMemberMapper.ToEntity)
            .ToList();;
        space.Roles = dto.Roles.Select(roleMapper.ToEntity).ToList();
        return space;
    }

    public SpaceDto ToDto(ISpace entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Members = entity.Members
                .Select(spaceMemberMapper.ToDto)
                .ToList(),
            Roles = entity.Roles.Select(roleMapper.ToDto).ToList()
        };
    }
}

public class SpaceShortMapper(
    IEntityFactory<ISpace> entityFactory)
    : IMapper<ISpace, SpaceDtoShort>
{
    public SpaceDtoShort ToDto(ISpace entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceDtoShort
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }

    public ISpace ToEntity(SpaceDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var space = entityFactory.CreateEntity();
        space.Id = dto.Id;
        space.Name = dto.Name;
        return space;
    }
}

