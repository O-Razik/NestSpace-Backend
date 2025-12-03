using EventScheduleService.ABS.IModels;
namespace EventScheduleService.ABS.IRepositories;

public interface ISoloEventRepository
{
    Task<IEnumerable<ISoloEvent>> GetBySpaceAsync(Guid spaceId);
    
    Task<ISoloEvent?> GetByIdAsync(Guid soloEventId);
    
    Task<ISoloEvent> AddAsync(ISoloEvent newSoloEvent);
    
    Task<ISoloEvent?> UpdateAsync(ISoloEvent updatedSoloEvent);
    
    Task<bool> DeleteAsync(Guid soloEventId);
    
    Task<bool> DeleteBySpaceIdAsync(Guid spaceId);
}