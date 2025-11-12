namespace ChatNotifyService.BLL.Dtos;

public class ChatMemberDtoShort
{
    public Guid ChatId { get; set; }
    
    public Guid MemberId { get; set; }
}

public class ChatMemberDto : ChatMemberDtoShort
{
    public DateTime JoinedAt { get; set; }
}