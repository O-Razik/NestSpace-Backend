namespace UserSpaceService.ABS.DTOs;

public class SpaceDtoShort
{
    public Guid Id { get; init; }
    
    public string Name { get; init; } = string.Empty;
}

public class SpaceDto : SpaceDtoShort
{
    public ICollection<SpaceMemberDto> Members { get; set; } = [];
    
    public ICollection<SpaceRoleDto> Roles { get; set; } = [];
}
