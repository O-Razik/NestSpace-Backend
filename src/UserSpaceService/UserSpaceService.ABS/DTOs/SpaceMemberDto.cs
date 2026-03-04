namespace UserSpaceService.ABS.DTOs;

public class SpaceMemberDtoShort
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
    
    public string? SpaceUsername { get; set; }
}

public class SpaceMemberDto : SpaceMemberDtoShort
{
    public SpaceRoleDto Role { get; set; } = new();
    
    public UserDtoShort User { get; set; } = new();
}

public class AddSpaceMemberDto
{
    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
}