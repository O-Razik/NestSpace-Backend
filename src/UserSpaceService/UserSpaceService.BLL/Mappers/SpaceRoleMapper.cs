using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public static class SpaceRoleMapper
{
    public static SpaceRole ToEntity(this SpaceRoleDto dto)
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

    public static SpaceRoleDto ToDto(this SpaceRole entity)
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
