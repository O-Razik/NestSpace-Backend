namespace UserSpaceService.ABS.Filters;

public class UserFilter
{
    public string SearchTerm { get; set; } = string.Empty;

    public bool SortDescending { get; set; }

    public int PageNumber { get; set; } = 1;
    
    public int PageSize { get; set; } = 20;
}   
