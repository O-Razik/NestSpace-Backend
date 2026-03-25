using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.RabbitMQ;

namespace EventScheduleService.BLL.Services;

public class RegularEventService(
    IRegularEventRepository regularEventRepository,
    SpaceLogPublish logPublish,
    IEntityMapper<IRegularEvent, CreateRegularEventDto> createMapper) : IRegularEventService
{
    public async Task<IEnumerable<IRegularEvent>> GetRegularEventsBySpaceAsync(Guid spaceId)
    {
        return await regularEventRepository.GetAllBySpaceAsync(spaceId);
    }

    public async Task<IRegularEvent?> GetRegularEventByIdAsync(Guid regularEventId)
    {
        return await regularEventRepository.GetByIdAsync(regularEventId);
    }

    public async Task<IRegularEvent> CreateRegularEventAsync(CreateRegularEventDto newRegularEvent)
    {
        var result = await regularEventRepository
            .AddAsync(createMapper.ToEntity(newRegularEvent));
          
        await logPublish.PublishSpaceActivityLogAsync(
            newRegularEvent.SpaceId, Guid.Empty, // MemberId can be added if available
            "RegularEventCreated", 
            $"Regular event '{newRegularEvent.Title}' created."
        );
            
        return result;
    }

    public async Task<IRegularEvent?> UpdateRegularEventAsync(IRegularEvent updatedRegularEvent)
    {
        var result = await regularEventRepository.UpdateAsync(updatedRegularEvent);

        if (result != null)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                updatedRegularEvent.SpaceId, Guid.Empty, // MemberId can be added if available
                "RegularEventUpdated",
                $"Regular event '{updatedRegularEvent.Title}' updated."
            );
        }
        return result;
    }

    public async Task<bool> DeleteRegularEventAsync(Guid regularEventId)
    {
        var result = await regularEventRepository.DeleteAsync(regularEventId);

        if (result)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                Guid.Empty, Guid.Empty, // SpaceId and MemberId can be added if available
                "RegularEventDeleted",
                $"Regular event with ID '{regularEventId}' deleted."
            );
        }
            
        return result;
    }

    public async Task<bool> DeleteRegularEventsBySpaceIdAsync(Guid spaceId)
    {
        return await regularEventRepository.DeleteBySpaceIdAsync(spaceId);
    }
}
