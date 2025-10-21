namespace UserSpaceService.BLL.DTOs;

public class SpaceDtoShort
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
}

public class SpaceDto : SpaceDtoShort
{
    public ICollection<SpaceMemberDto> Members { get; set; } = new List<SpaceMemberDto>();
    
    public ICollection<SpaceRoleDto> Roles { get; set; } = new List<SpaceRoleDto>();
}