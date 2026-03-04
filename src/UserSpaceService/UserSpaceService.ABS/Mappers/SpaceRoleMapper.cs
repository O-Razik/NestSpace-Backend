using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.Mappers;

public class SpaceRoleMapper(IEntityFactory<ISpaceRole> entityFactory)
    : IMapper<ISpaceRole, SpaceRoleDto>
{
    public ISpaceRole ToEntity(SpaceRoleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var spaceRole = entityFactory.CreateEntity();
        spaceRole.Id = dto.Id;
        spaceRole.SpaceId = dto.SpaceId;
        spaceRole.Name = dto.Name;
        spaceRole.RolePermissions = dto.RolePermissions;
        return spaceRole;
    }

    public SpaceRoleDto ToDto(ISpaceRole entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceRoleDto
        {
            Id = entity.Id,
            SpaceId = entity.SpaceId,
            Name = entity.Name,
            RolePermissions = entity.RolePermissions,
        };
    }
}