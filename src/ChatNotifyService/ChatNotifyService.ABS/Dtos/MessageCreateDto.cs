namespace ChatNotifyService.ABS.Dtos;

public class MessageCreateDto
{
    public Guid ChatId { get; set; }
    
    public Guid SenderId { get; set; }
    
    public Guid? ReplyToMessageId { get; set; }
    
    public string Content { get; set; } = string.Empty;
}
