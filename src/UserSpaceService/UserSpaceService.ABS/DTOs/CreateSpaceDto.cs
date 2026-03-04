namespace UserSpaceService.ABS.DTOs;

public class CreateSpaceDto
{
    public string Name { get; set; } = string.Empty;
    
    public List<Guid> MemberIds { get; set; } = [];
}