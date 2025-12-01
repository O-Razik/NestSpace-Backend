namespace ChatNotifyService.BLL.Dtos.Send;

public class ChatDtoShort
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public string Name { get; set; } = string.Empty;
}

public class ChatDto : ChatDtoShort
{
    public ICollection<MemberDto> Members { get; set; } = new List<MemberDto>();
}