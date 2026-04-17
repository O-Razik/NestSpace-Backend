using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public class SpaceRoleMapper
    : IMapper<SpaceRole, SpaceRoleDto>
{
    public SpaceRole ToEntity(SpaceRoleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new SpaceRole
        {
            Id = dto.Id,
            SpaceId = dto.SpaceId,
            Name = dto.Name,
            RolePermissions = dto.RolePermissions,
        };
    }

    public SpaceRoleDto ToDto(SpaceRole entity)
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
