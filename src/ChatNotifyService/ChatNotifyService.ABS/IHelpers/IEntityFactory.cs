namespace ChatNotifyService.ABS.IHelpers;

public interface IEntityFactory<T>
{
    T CreateEntity();
}