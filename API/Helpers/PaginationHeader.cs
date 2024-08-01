namespace API.Helpers;

public class PaginationHeader(int currentPage, int totalPages,
 int itemsPerPage, int totalItems)
{
    public int CurrentPage { get; set; } = currentPage;

    public int TotalPages { get; set; } = totalPages;

    public int ItemsPerPage { get; set; } = itemsPerPage;

    public int TotalItems { get; set; } = totalItems;
}
