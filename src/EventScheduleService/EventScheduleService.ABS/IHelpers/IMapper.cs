namespace EventScheduleService.ABS.IHelpers;

public interface IMapper<TSource, TDto>
{
    TDto ToDto(TSource source);
    
    TSource ToEntity(TDto dto);
}

public interface IEntityMapper<TSource, TDto>
{
    TSource ToEntity(TDto dto);
}
