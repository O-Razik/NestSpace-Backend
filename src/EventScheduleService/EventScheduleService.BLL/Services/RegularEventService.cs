using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;

namespace EventScheduleService.BLL.Services;

public class RegularEventService(
    IRegularEventRepository regularEventRepository
) : IRegularEventService
{
    public async Task<IEnumerable<IRegularEvent>> GetRegularEventsBySpaceAsync(Guid spaceId)
    {
        return await regularEventRepository.GetAllBySpaceAsync(spaceId);
    }

    public async Task<IRegularEvent?> GetRegularEventByIdAsync(Guid regularEventId)
    {
        return await regularEventRepository.GetByIdAsync(regularEventId);
    }

    public async Task<IRegularEvent> CreateRegularEventAsync(IRegularEvent newRegularEvent)
    {
        try
        {
            return await regularEventRepository.AddAsync(newRegularEvent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IRegularEvent?> UpdateRegularEventAsync(IRegularEvent updatedRegularEvent)
    {
        try
        {
            return await regularEventRepository.UpdateAsync(updatedRegularEvent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteRegularEventAsync(Guid regularEventId)
    {
        try
        {
            return await regularEventRepository.DeleteAsync(regularEventId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteRegularEventsBySpaceIdAsync(Guid spaceId)
    {
        try
        {
            return await regularEventRepository.DeleteBySpaceIdAsync(spaceId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}