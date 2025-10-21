namespace UserSpaceService.ABS.IHelpers;

public interface IEntityFactory<T>
{
    T CreateEntity();
}