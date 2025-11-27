using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IServices;

public interface IRegularEventService
{
    Task<IEnumerable<IRegularEvent>> GetRegularEventsBySpaceAsync(Guid spaceId);
    
    Task<IRegularEvent?> GetRegularEventByIdAsync(Guid regularEventId);
    
    Task<IRegularEvent> CreateRegularEventAsync(IRegularEvent newRegularEvent);
    
    Task<IRegularEvent?> UpdateRegularEventAsync(IRegularEvent updatedRegularEvent);
    
    Task<bool> DeleteRegularEventAsync(Guid regularEventId);
}