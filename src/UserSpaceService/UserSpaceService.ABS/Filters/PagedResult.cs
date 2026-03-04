namespace UserSpaceService.ABS.Filters;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    
    public int PageCount { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
}