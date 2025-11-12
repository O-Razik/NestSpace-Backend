using ChatNotifyService.ABS.IEntities;

namespace ChatNotifyService.ABS.IRepositories;

public interface IMessageReadRepository
{
    Task<IEnumerable<IMessageRead>> GetAllAsync(Guid messageId);

    Task<IMessageRead?> GetByIdAsync(Guid messageId, Guid readerId);

    Task<IMessageRead> MarkAsReadAsync(Guid messageId, Guid readerId);
    
    Task<IEnumerable<IMessageRead>> MarkAsReadsAsync(IEnumerable<IMessage> messages, Guid readerId);

    Task<bool> ExistsAsync(Guid messageId, Guid readerId);
}