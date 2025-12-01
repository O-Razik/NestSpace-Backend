namespace ChatNotifyService.BLL.Dtos.Send;

public class MessageReadDtoShort
{
    public Guid MessageId { get; set; }
    
    public Guid ReaderId { get; set; }
    
    public DateTime ReadAt { get; set; }
}

public class MessageReadDto : MessageReadDtoShort
{
    public MemberDto Reader { get; set; } = null!;
}