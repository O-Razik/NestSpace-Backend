using ChatNotifyService.ABS.Models;

namespace ChatNotifyService.ABS.IRepositories;

public interface IMessageReadRepository
{
    Task<IEnumerable<MessageRead>> GetAllAsync(Guid messageId);

    Task<MessageRead?> GetByIdAsync(Guid messageId, Guid readerId);

    Task<MessageRead> MarkAsReadAsync(Guid messageId, Guid readerId);
    
    Task<IEnumerable<MessageRead>> MarkAsReadsAsync(IEnumerable<Message> messages, Guid readerId);

    Task<bool> ExistsAsync(Guid messageId, Guid readerId);
}
