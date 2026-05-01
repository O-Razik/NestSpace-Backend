namespace UserSpaceService.ABS.Dtos;

public class SpaceMemberDtoShort
{
    // 🔧 Composite Key: (SpaceId, UserId) - нема окремого .Id
    public Guid SpaceId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public string? SpaceUsername { get; set; }

    public Guid? SubgroupId { get; set; }

    public DateTimeOffset JoinedAt { get; set; }
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
