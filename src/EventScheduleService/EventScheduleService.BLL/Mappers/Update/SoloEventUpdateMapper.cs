using EventScheduleService.ABS.IHelpers;
using EventScheduleService.ABS.IModels;
using EventScheduleService.BLL.Dto.Update;

namespace EventScheduleService.BLL.Mappers.Update;

public class SoloEventUpdateMapper : ICreateMapper<ISoloEvent, SoloEventUpdateDto>
{
    public ISoloEvent ToEntity(Guid spaceId, SoloEventUpdateDto dto)
    {
        var entity = Activator.CreateInstance<ISoloEvent>();
        entity.Id = dto.Id;
        entity.SpaceId = spaceId;
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsYearly = dto.IsYearly;
        return entity;
    }
}