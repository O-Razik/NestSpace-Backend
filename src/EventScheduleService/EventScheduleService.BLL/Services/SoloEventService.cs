using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;

namespace EventScheduleService.BLL.Services;

public class SoloEventService(
    ISoloEventRepository soloEventRepository
) : ISoloEventService
{
    public async Task<IEnumerable<ISoloEvent>> GetSoloEventsBySpaceAsync(Guid spaceId)
    {
        return await soloEventRepository.GetBySpaceAsync(spaceId);
    }

    public async Task<ISoloEvent?> GetSoloEventByIdAsync(Guid soloEventId)
    {
        return await soloEventRepository.GetByIdAsync(soloEventId);
    }

    public async Task<ISoloEvent> CreateSoloEventAsync(ISoloEvent newSoloEvent)
    {
        try
        {
            return await soloEventRepository.AddAsync(newSoloEvent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ISoloEvent?> UpdateSoloEventAsync(ISoloEvent updatedSoloEvent)
    {
        try
        {
            return await soloEventRepository.UpdateAsync(updatedSoloEvent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteSoloEventAsync(Guid soloEventId)
    {
        try
        {
            return await soloEventRepository.DeleteAsync(soloEventId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}