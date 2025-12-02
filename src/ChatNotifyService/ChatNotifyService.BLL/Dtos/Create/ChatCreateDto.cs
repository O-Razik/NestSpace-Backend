namespace ChatNotifyService.BLL.Dtos.Create;

public class ChatCreateDto
{
    Guid SpaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<MemberCreateDto> Members { get; set; } = new List<MemberCreateDto>();
}