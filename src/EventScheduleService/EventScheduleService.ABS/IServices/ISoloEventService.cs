using EventScheduleService.ABS.Dtos;
using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IServices;

public interface ISoloEventService
{
    Task<IEnumerable<SoloEvent>> GetSoloEventsBySpaceAsync(Guid spaceId);
    
    Task<SoloEvent?> GetSoloEventByIdAsync(Guid soloEventId);
    
    Task<SoloEvent> CreateSoloEventAsync(SoloEventCreateDto newSoloEvent);
    
    Task<SoloEvent?> UpdateSoloEventAsync(SoloEvent updatedSoloEvent);
    
    Task<bool> DeleteSoloEventAsync(Guid soloEventId);
    
    Task<bool> DeleteSoloEventsBySpaceIdAsync(Guid spaceId);
}
