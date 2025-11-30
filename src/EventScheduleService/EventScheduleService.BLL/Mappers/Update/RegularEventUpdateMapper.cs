using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Update;

namespace EventScheduleService.BLL.Mappers.Update;

public class RegularEventUpdateMapper : ICreateMapper<IRegularEvent, RegularEventUpdateDto>
{
    public IRegularEvent ToEntity(Guid spaceId, RegularEventUpdateDto dto)
    {
        var entity = Activator.CreateInstance<IRegularEvent>();
        entity.Id = dto.Id;
        entity.SpaceId = spaceId;
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.StartTime = dto.StartTime;
        entity.Duration = dto.Duration;
        entity.Day = dto.Day;
        entity.Frequency = dto.Frequency;
        return entity;
    }
}