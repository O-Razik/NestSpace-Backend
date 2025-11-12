namespace ChatNotifyService.ABS.IEntities;

public interface IChat
{
    Guid Id { get; set; }
    
    Guid SpaceId { get; set; }
    
    string Name { get; set; }
    
    ICollection<IChatMember> Members { get; set; }
}