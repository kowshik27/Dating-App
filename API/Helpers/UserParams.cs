namespace API.Helpers;

public class UserParams
{
    public int PageNumber { get; set; } = 1;

    private const int MaxPageSize = 10;

    private int _pageSize = 5;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string? Gender { get; set; }

    public string? CurrentUsername { get; set; }

    public int MinAge { get; set; } = 18;
    
    public int MaxAge { get; set; } = 50;

    public string OrderBy { get; set; } = "lastActive";
    

}
