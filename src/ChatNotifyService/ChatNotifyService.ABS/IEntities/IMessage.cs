namespace ChatNotifyService.ABS.IEntities;

public interface IMessage
{
    Guid Id { get; set; }
    
    Guid ChatId { get; set; }
    
    Guid SenderId { get; set; }
    
    string Content { get; set; }
    
    DateTime SentAt { get; set; }
    
    DateTime? ModifiedAt { get; set; }
    
    bool IsEdited { get; set; }
    
    bool IsDeleted { get; set;}
    
    Guid? ReplyToMessageId { get; set; }
    
    IChatMember Sender { get; set; }

    ICollection<IMessageRead> Reads { get; set; }
}