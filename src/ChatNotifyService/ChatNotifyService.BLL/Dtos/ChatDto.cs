namespace ChatNotifyService.BLL.Dtos;

public class ChatDtoShort
{
    public Guid Id { get; set; }
    
    public Guid SpaceId { get; set; }
    
    public string Name { get; set; } = string.Empty;
}

public class ChatDto : ChatDtoShort
{
    public ICollection<ChatMemberDto> Members { get; set; } = new List<ChatMemberDto>();
}