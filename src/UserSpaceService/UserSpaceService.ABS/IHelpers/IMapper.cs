namespace UserSpaceService.ABS.IHelpers;

public interface IMapper<TSource, TDto>
{
    TDto ToDto(TSource source);
    
    TSource ToEntity(TDto dto);
}
