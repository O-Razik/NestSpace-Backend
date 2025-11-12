namespace ChatNotifyService.BLL.Dtos;

public class MessageReadDtoShort
{
    public Guid MessageId { get; set; }
    
    public Guid ReaderId { get; set; }
    
    public DateTime ReadAt { get; set; }
}

public class MessageReadDto : MessageReadDtoShort
{
    public ChatMemberDto Reader { get; set; } = null!;
}