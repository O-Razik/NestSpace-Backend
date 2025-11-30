using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IRepositories;

public interface IRegularEventRepository
{
    Task<IEnumerable<IRegularEvent>> GetAllBySpaceAsync(Guid spaceId);
    
    Task<IRegularEvent?> GetByIdAsync(Guid regularEventId);
    
    Task<IRegularEvent> AddAsync(IRegularEvent newRegularEvent);
    
    Task<IRegularEvent?> UpdateAsync(IRegularEvent updatedRegularEvent);
    
    Task<bool> DeleteAsync(Guid regularEventId);
}