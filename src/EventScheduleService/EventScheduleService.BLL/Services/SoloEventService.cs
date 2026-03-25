using EventScheduleService.ABS.Dto;
using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.Models;
using EventScheduleService.ABS.IRepositories;
using EventScheduleService.ABS.IServices;
using EventScheduleService.BLL.RabbitMQ;

namespace EventScheduleService.BLL.Services;

public class SoloEventService(
    ISoloEventRepository soloEventRepository,
    SpaceLogPublish logPublish,
    IEntityMapper<SoloEvent, CreateSoloEventDto> createMapper) : ISoloEventService
{
    public async Task<IEnumerable<SoloEvent>> GetSoloEventsBySpaceAsync(Guid spaceId)
    {
        return await soloEventRepository.GetBySpaceAsync(spaceId);
    }

    public async Task<SoloEvent?> GetSoloEventByIdAsync(Guid soloEventId)
    {
        return await soloEventRepository.GetByIdAsync(soloEventId);
    }

    public async Task<SoloEvent> CreateSoloEventAsync(CreateSoloEventDto newSoloEvent)
    {
        var result = await soloEventRepository
            .AddAsync(createMapper.ToEntity(newSoloEvent));
            
        await logPublish.PublishSpaceActivityLogAsync(
            newSoloEvent.SpaceId, Guid.Empty, // MemberId can be added if available
            "SoloEventCreated", 
            $"Solo event '{newSoloEvent.Title}' created."
        );
            
        return result;
    }

    public async Task<SoloEvent?> UpdateSoloEventAsync(SoloEvent updatedSoloEvent)
    {
        var result = await soloEventRepository.UpdateAsync(updatedSoloEvent);
            
        if (result != null)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                updatedSoloEvent.SpaceId, Guid.Empty, // MemberId can be added if available
                "SoloEventUpdated",
                $"Solo event '{updatedSoloEvent.Title}' updated."
            );
        }
            
        return result;
    }

    public async Task<bool> DeleteSoloEventAsync(Guid soloEventId)
    {
        var result = await soloEventRepository.DeleteAsync(soloEventId);
        if (result)
        {
            await logPublish.PublishSpaceActivityLogAsync(
                Guid.Empty, Guid.Empty, // SpaceId and MemberId can be added if available
                "SoloEventDeleted",
                $"Solo event with ID '{soloEventId}' deleted."
            );
        }
            
        return result;
    }

    public async Task<bool> DeleteSoloEventsBySpaceIdAsync(Guid spaceId)
    {
        return await soloEventRepository.DeleteBySpaceIdAsync(spaceId);
    }
}
