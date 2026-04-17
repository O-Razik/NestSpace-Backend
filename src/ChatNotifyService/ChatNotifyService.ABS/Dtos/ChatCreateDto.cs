namespace ChatNotifyService.ABS.Dtos;

public class ChatCreateDto
{
    public Guid SpaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<MemberCreateDto> Members { get; set; } = new List<MemberCreateDto>();
}
