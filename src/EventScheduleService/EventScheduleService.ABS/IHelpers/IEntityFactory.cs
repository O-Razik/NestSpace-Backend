namespace EventScheduleService.ABS.IHelpers;

public interface IEntityFactory<T>
{
    T CreateEntity();
}