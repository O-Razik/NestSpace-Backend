namespace UserSpaceService.ABS.DTOs;

public class CreateSpaceDto
{
    public Guid CreatorId { get; set; } = Guid.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public IList<Guid> MemberIds { get; } = [];
}
