using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IServices;

public interface IRegularEventService
{
    Task<IEnumerable<RegularEvent>> GetRegularEventsBySpaceAsync(Guid spaceId);
    
    Task<RegularEvent?> GetRegularEventByIdAsync(Guid regularEventId);
    
    Task<RegularEvent> CreateRegularEventAsync(CreateRegularEventDto newRegularEvent);
    
    Task<RegularEvent?> UpdateRegularEventAsync(RegularEvent updatedRegularEvent);
    
    Task<bool> DeleteRegularEventAsync(Guid regularEventId);
    
    Task<bool> DeleteRegularEventsBySpaceIdAsync(Guid spaceId);
}