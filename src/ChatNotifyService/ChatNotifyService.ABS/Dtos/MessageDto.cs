namespace ChatNotifyService.ABS.Dtos;

public class MessageDtoShort
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    
    public Guid SenderId { get; set; }
    
    public string Content { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public bool IsEdited { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public Guid? ReplyToMessageId { get; set; }
}

public class MessageDto : MessageDtoShort
{
    public MemberDtoShort Sender { get; set; } = null!;
    
    public ICollection<MessageReadDto> Reads { get; set; } = new List<MessageReadDto>();
}
