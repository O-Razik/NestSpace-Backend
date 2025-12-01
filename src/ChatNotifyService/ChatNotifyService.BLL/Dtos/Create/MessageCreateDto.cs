namespace ChatNotifyService.BLL.Dtos.Create;

public class MessageCreateDto
{
    public Guid SenderId { get; set; }
    
    public Guid? ReplyToMessageId { get; set; }
    
    public string Content { get; set; } = string.Empty;
}