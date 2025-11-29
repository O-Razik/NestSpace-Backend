namespace EventScheduleService.ABS.IHelpers;

public interface IMapper<TSource, TDto>
{
    TDto ToDto(TSource source);
    
    TSource ToEntity(TDto dto);
}

public interface IMapper<TSource, TDto, TShortDto>
{
    TDto ToDto(TSource source);
    
    TSource ToEntity(TDto dto);
    
    TShortDto ToShortDto(TSource source);
    
    TSource ToEntity(TShortDto dto);
}

public interface IShortMapper<out TSource, in TDto>
{
    TSource ToEntity(TDto dto);
}