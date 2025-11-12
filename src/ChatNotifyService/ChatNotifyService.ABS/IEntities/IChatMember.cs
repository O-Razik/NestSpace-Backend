namespace ChatNotifyService.ABS.IEntities;

public interface IChatMember
{
    Guid ChatId { get; set; }
    
    Guid MemberId { get; set; }
    
    DateTime JoinedAt { get; set; }
}