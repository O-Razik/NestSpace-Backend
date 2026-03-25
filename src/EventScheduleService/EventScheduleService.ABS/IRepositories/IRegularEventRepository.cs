using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IRepositories;

public interface IRegularEventRepository
{
    Task<IEnumerable<RegularEvent>> GetAllBySpaceAsync(Guid spaceId);
    
    Task<RegularEvent?> GetByIdAsync(Guid regularEventId);
    
    Task<RegularEvent> AddAsync(RegularEvent newRegularEvent);
    
    Task<RegularEvent?> UpdateAsync(RegularEvent updatedRegularEvent);
    
    Task<bool> DeleteAsync(Guid regularEventId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}
