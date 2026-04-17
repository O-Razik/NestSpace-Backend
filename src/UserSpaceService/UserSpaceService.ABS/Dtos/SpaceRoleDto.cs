using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.Dtos;

public class SpaceRoleDto : SpaceRoleDtoShort
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
}

public class SpaceRoleDtoShort
{
    public string Name { get; set; } = string.Empty;
    
    public Permission RolePermissions { get; set; }
}