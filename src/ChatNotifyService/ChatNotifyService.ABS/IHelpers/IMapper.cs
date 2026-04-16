namespace ChatNotifyService.ABS.IHelpers;

public interface IMapper<TSource, TDto>
{
    TDto ToDto(TSource source);
    
    TSource ToEntity(TDto dto);
}

public interface IBigMapper<TSource, TDto, TShortDto> : IMapper<TSource, TDto>
{
    TShortDto ToShortDto(TSource source);
    TSource ToEntity(TShortDto dto);
}

public interface ICreateMapper<out TSource, in TDto>
{
    TSource ToEntity(TDto dto);
}
