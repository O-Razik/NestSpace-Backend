namespace UserSpaceService.ABS.Filters;

public class SpaceFilter
{
    public string SearchTerm { get; set; } = string.Empty;
    
    public SpaceSortBy SortBy { get; set; } = SpaceSortBy.NameDescending;
    
    public int PageNumber { get; set; } = 1;
    
    public int PageSize { get; set; } = 20;
}

public enum SpaceSortBy
{
    NameDescending,
    NameAscending,
    MemberCountDescending,
    MemberCountAscending
}
