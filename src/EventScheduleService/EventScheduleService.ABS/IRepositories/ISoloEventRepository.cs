using EventScheduleService.ABS.Models;

namespace EventScheduleService.ABS.IRepositories;

public interface ISoloEventRepository
{
    Task<IEnumerable<SoloEvent>> GetBySpaceAsync(Guid spaceId);
    
    Task<SoloEvent?> GetByIdAsync(Guid soloEventId);
    
    Task<SoloEvent> AddAsync(SoloEvent newSoloEvent);
    
    Task<SoloEvent?> UpdateAsync(SoloEvent updatedSoloEvent);
    
    Task<bool> DeleteAsync(Guid soloEventId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}
