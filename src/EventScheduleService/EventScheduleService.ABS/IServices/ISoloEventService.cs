using EventScheduleService.ABS.IModels;

namespace EventScheduleService.ABS.IServices;

public interface ISoloEventService
{
    Task<IEnumerable<ISoloEvent>> GetSoloEventsBySpaceAsync(Guid spaceId);
    
    Task<ISoloEvent?> GetSoloEventByIdAsync(Guid soloEventId);
    
    Task<ISoloEvent> CreateSoloEventAsync(ISoloEvent newSoloEvent);
    
    Task<ISoloEvent?> UpdateSoloEventAsync(ISoloEvent updatedSoloEvent);
    
    Task<bool> DeleteSoloEventAsync(Guid soloEventId);
}