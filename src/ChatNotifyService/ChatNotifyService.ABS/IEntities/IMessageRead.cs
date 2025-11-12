namespace ChatNotifyService.ABS.IEntities;

public interface IMessageRead
{
    Guid MessageId { get; set; }
    
    Guid ReaderId { get; set; }
    
    DateTime ReadAt { get; set; }
    
    IChatMember Reader { get; set; }
}